namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class IsVisibleExtensions
    {
        public static bool IsVisible(this TypeDeclarationSyntax node, ITypeDefinition? declaringType)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            if (declaringType != null
                && declaringType.IsVisible == false)
            {
                // The parent type is not visible so this one can't be either
                return false;
            }

            // This is either a top level type or the parent type is visible
            // Determine visibility based on the access modifier
            var accessModifier = node.DetermineAccessModifier(declaringType);

            return accessModifier.IsVisible();
        }

        public static bool IsVisible(this AccessModifier modifier)
        {
            switch (modifier)
            {
                case AccessModifier.None:
                    throw new InvalidOperationException("The modifier must be specified to determine the visibility");
                case AccessModifier.Internal:
                case AccessModifier.Private:
                    return false;
                case AccessModifier.ProtectedPrivate:
                case AccessModifier.ProtectedInternal:
                case AccessModifier.Protected:
                case AccessModifier.Public:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(modifier), modifier, null);
            }
        }

        public static bool IsVisible(this AccessorDeclarationSyntax declaration, IPropertyDefinition declaringProperty)
        {
            declaration = declaration ?? throw new ArgumentNullException(nameof(declaration));

            if (declaringProperty.IsVisible == false)
            {
                return false;
            }

            var modifier = declaration.DetermineAccessModifier(declaringProperty);

            return modifier.IsVisible();
        }

        public static bool IsVisible(this MemberDeclarationSyntax node, ITypeDefinition declaringType)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            if (declaringType.IsVisible == false)
            {
                // The parent type is not visible so this one can't be either
                return false;
            }

            var accessModifier = node.DetermineAccessModifier(declaringType);

            return accessModifier.IsVisible();
        }
    }
}