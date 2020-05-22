namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class IsVisibleExtensions
    {
        public static bool IsVisible(this AccessorDeclarationSyntax declaration)
        {
            if (declaration == null)
            {
                throw new ArgumentNullException(nameof(declaration));
            }

            var modifiers = declaration.Modifiers;

            return HasVisibleModifiers(modifiers);
        }

        public static bool IsVisible(this ClassDeclarationSyntax declaration)
        {
            if (declaration == null)
            {
                throw new ArgumentNullException(nameof(declaration));
            }

            var modifiers = declaration.Modifiers;

            return HasVisibleModifiers(modifiers);
        }

        public static bool IsVisible(this InterfaceDeclarationSyntax declaration)
        {
            if (declaration == null)
            {
                throw new ArgumentNullException(nameof(declaration));
            }

            var modifiers = declaration.Modifiers;

            return HasVisibleModifiers(modifiers);
        }

        public static bool IsVisible(this MemberDeclarationSyntax declaration)
        {
            if (declaration == null)
            {
                throw new ArgumentNullException(nameof(declaration));
            }

            var modifiers = declaration.Modifiers;

            return HasVisibleModifiers(modifiers);
        }

        private static bool HasVisibleModifiers(SyntaxTokenList modifiers)
        {
            foreach (var modifier in modifiers)
            {
                var text = modifier.Text;

                if (text == "public")
                {
                    return true;
                }

                if (text == "protected")
                {
                    return true;
                }
            }

            return false;
        }
    }
}