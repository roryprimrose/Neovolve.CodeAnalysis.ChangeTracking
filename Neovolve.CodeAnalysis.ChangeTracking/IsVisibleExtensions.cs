namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class IsVisibleExtensions
    {
        public static bool IsVisible(this AccessorDeclarationSyntax declaration)
        {
            declaration = declaration ?? throw new ArgumentNullException(nameof(declaration));

            var modifiers = declaration.Modifiers;

            return HasVisibleModifiers(modifiers);
        }

        public static bool IsVisible(this MemberDeclarationSyntax declaration)
        {
            declaration = declaration ?? throw new ArgumentNullException(nameof(declaration));

            if (declaration.Parent is InterfaceDeclarationSyntax
                && declaration.Modifiers.Count == 0)
            {
                // Interfaces members without modifiers inherit the parent access modifier
                // As IsVisible here is about the member not the interface, assume the member is visible
                return true;
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