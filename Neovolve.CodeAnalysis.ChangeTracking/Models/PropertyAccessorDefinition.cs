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

            AccessModifiers = DetermineAccessModifier(node);
            IsVisible = DetermineIsVisible(declaringProperty, AccessModifiers);

            var nameSuffix = AccessorTypeName(node);
            var name = declaringProperty.Name + nameSuffix;

            AccessorType = GetAccessorType(node);
            AccessorPurpose = GetAccessorPurpose(node);
            Name = name;
            FullName = declaringProperty.FullName + nameSuffix;
            FullRawName = declaringProperty.FullRawName + nameSuffix;
            RawName = declaringProperty.RawName + nameSuffix;
        }

        public override bool Matches(IElementDefinition element, ElementMatchOptions options)
        {
            if (GetType() != element.GetType())
            {
                return false;
            }

            var item = (IPropertyAccessorDefinition)element;
            
            if (AccessorType != item.AccessorType)
            {
                return false;
            }

            if (options.HasFlag(ElementMatchOptions.IgnoreName) == false
                && Name != item.Name)
            {
                return false;
            }

            return true;
        }

        private static string AccessorTypeName(AccessorDeclarationSyntax node)
        {
            var kind = node.Kind();

            if (kind == SyntaxKind.SetAccessorDeclaration)
            {
                return "_set";
            }

            if (kind == SyntaxKind.InitAccessorDeclaration)
            {
                return "_init";
            }

            return "_get";
        }

        private static PropertyAccessorAccessModifiers DetermineAccessModifier(AccessorDeclarationSyntax node)
        {
            // Check this one first as it is the most common
            if (node.Modifiers.HasModifier(SyntaxKind.PrivateKeyword))
            {
                return PropertyAccessorAccessModifiers.Private;
            }

            if (node.Modifiers.HasModifier(SyntaxKind.ProtectedKeyword))
            {
                if (node.Modifiers.HasModifier(SyntaxKind.InternalKeyword))
                {
                    return PropertyAccessorAccessModifiers.ProtectedInternal;
                }

                return PropertyAccessorAccessModifiers.Protected;
            }

            if (node.Modifiers.HasModifier(SyntaxKind.InternalKeyword))
            {
                return PropertyAccessorAccessModifiers.Internal;
            }

            return PropertyAccessorAccessModifiers.None;
        }

        private static bool DetermineIsVisible(IPropertyDefinition propertyDefinition,
            PropertyAccessorAccessModifiers accessModifiers)
        {
            if (propertyDefinition.IsVisible == false)
            {
                // The parent property is not visible so neither is the accessor
                return false;
            }

            if (accessModifiers == PropertyAccessorAccessModifiers.None)
            {
                // There is no access modifiers defined to further restrict visibility so we inherit from the declaring property
                return true;
            }

            if (accessModifiers == PropertyAccessorAccessModifiers.Protected)
            {
                return true;
            }

            if (accessModifiers == PropertyAccessorAccessModifiers.ProtectedInternal)
            {
                return true;
            }

            return false;
        }

        private static PropertyAccessorPurpose GetAccessorPurpose(AccessorDeclarationSyntax node)
        {
            var kind = node.Kind();

            if (kind == SyntaxKind.GetAccessorDeclaration)
            {
                return PropertyAccessorPurpose.Read;
            }

            return PropertyAccessorPurpose.Write;
        }

        private static PropertyAccessorType GetAccessorType(AccessorDeclarationSyntax node)
        {
            var kind = node.Kind();

            if (kind == SyntaxKind.SetAccessorDeclaration)
            {
                return PropertyAccessorType.Set;
            }

            if (kind == SyntaxKind.InitAccessorDeclaration)
            {
                return PropertyAccessorType.Init;
            }

            return PropertyAccessorType.Get;
        }

        public PropertyAccessorAccessModifiers AccessModifiers { get; }
        public PropertyAccessorPurpose AccessorPurpose { get; }
        public PropertyAccessorType AccessorType { get; }
        public override string FullName { get; }
        public override string FullRawName { get; }
        public override string Name { get; }
        public override string RawName { get; }
    }
}