﻿namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

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
    }
}