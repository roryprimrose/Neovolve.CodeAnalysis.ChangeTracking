namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class AccessModifierExtensions
    {
        public static AccessModifier DetermineAccessModifier(this TypeDeclarationSyntax node,
            ITypeDefinition? declaringType)
        {
            if (declaringType == null)
            {
                return DetermineAccessModifier(node.Modifiers, AccessModifier.Internal);
            }

            return DetermineAccessModifier(node.Modifiers, AccessModifier.Private);
        }

        public static AccessModifier DetermineAccessModifier(this AccessorDeclarationSyntax node,
            IPropertyDefinition declaringProperty)
        {
            return DetermineAccessModifier(node.Modifiers, declaringProperty.AccessModifier);
        }

        public static AccessModifier DetermineAccessModifier(this MemberDeclarationSyntax node,
            ITypeDefinition declaringType)
        {
            declaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));

            // See default values as identified at https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/accessibility-levels
            if (declaringType is IInterfaceDefinition)
            {
                return DetermineAccessModifier(node.Modifiers, AccessModifier.Public);
            }

            if (declaringType is IClassDefinition)
            {
                return DetermineAccessModifier(node.Modifiers, AccessModifier.Private);
            }

            // TODO: Fill these out when the types are supported
            throw new NotSupportedException();

            // Struct default access modifier is private
            // Struct default enum is public
        }

        private static AccessModifier DetermineAccessModifier(SyntaxTokenList tokenList,
            AccessModifier defaultValue)
        {
            if (tokenList.HasModifier(SyntaxKind.ProtectedKeyword))
            {
                if (tokenList.HasModifier(SyntaxKind.InternalKeyword))
                {
                    return AccessModifier.ProtectedInternal;
                }

                if (tokenList.HasModifier(SyntaxKind.PrivateKeyword))
                {
                    return AccessModifier.ProtectedPrivate;
                }

                return AccessModifier.Protected;
            }

            if (tokenList.HasModifier(SyntaxKind.InternalKeyword))
            {
                return AccessModifier.Internal;
            }

            if (tokenList.HasModifier(SyntaxKind.PrivateKeyword))
            {
                return AccessModifier.Private;
            }

            if (tokenList.HasModifier(SyntaxKind.PublicKeyword))
            {
                return AccessModifier.Public;
            }

            return defaultValue;
        }
    }
}