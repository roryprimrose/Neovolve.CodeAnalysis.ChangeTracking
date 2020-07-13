namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class PropertyAccessorDefinition : ElementDefinition, IPropertyAccessorDefinition
    {
        public PropertyAccessorDefinition(IPropertyDefinition declaringProperty, AccessorDeclarationSyntax node) :
            base(node)
        {
            declaringProperty = declaringProperty ?? throw new ArgumentNullException(nameof(declaringProperty));

            AccessModifier = DetermineAccessModifier(node);
            IsVisible = DetermineIsVisible(declaringProperty, AccessModifier);

            var nameSuffix = node.Kind() == SyntaxKind.GetAccessorDeclaration ? "_get" : "_set";
            var name = declaringProperty.Name + nameSuffix;

            Name = name;
            Description = $"Property accessor {name}";
            FullName = declaringProperty.FullName + nameSuffix;
            FullRawName = declaringProperty.FullRawName + nameSuffix;
            RawName = declaringProperty.RawName + nameSuffix;
        }

        private static PropertyAccessorAccessModifier DetermineAccessModifier(AccessorDeclarationSyntax node)
        {
            // Check this one first as it is the most common
            if (node.Modifiers.HasModifier(SyntaxKind.PrivateKeyword))
            {
                return PropertyAccessorAccessModifier.Private;
            }

            if (node.Modifiers.HasModifier(SyntaxKind.ProtectedKeyword))
            {
                if (node.Modifiers.HasModifier(SyntaxKind.InternalKeyword))
                {
                    return PropertyAccessorAccessModifier.ProtectedInternal;
                }

                return PropertyAccessorAccessModifier.Protected;
            }

            if (node.Modifiers.HasModifier(SyntaxKind.InternalKeyword))
            {
                return PropertyAccessorAccessModifier.Internal;
            }

            return PropertyAccessorAccessModifier.None;
        }

        private static bool DetermineIsVisible(IPropertyDefinition propertyDefinition,
            PropertyAccessorAccessModifier accessModifier)
        {
            if (propertyDefinition.IsVisible == false)
            {
                // The parent property is not visible so neither is the accessor
                return false;
            }

            if (accessModifier == PropertyAccessorAccessModifier.None)
            {
                // There is no access modifier defined to further restrict visibility so we inherit from the declaring property
                return true;
            }

            if (accessModifier == PropertyAccessorAccessModifier.Protected)
            {
                return true;
            }

            if (accessModifier == PropertyAccessorAccessModifier.ProtectedInternal)
            {
                return true;
            }

            return false;
        }

        public PropertyAccessorAccessModifier AccessModifier { get; }
        public override string Description { get; }
        public override string FullName { get; }
        public override string FullRawName { get; }
        public override bool IsVisible { get; }
        public override string Name { get; }
        public override string RawName { get; }
    }
}