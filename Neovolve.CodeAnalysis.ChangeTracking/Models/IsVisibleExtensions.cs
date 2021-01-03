namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;

    public static class IsVisibleExtensions
    {
        public static bool IsVisible(this AccessModifiers modifiers)
        {
            switch (modifiers)
            {
                case AccessModifiers.Internal:
                case AccessModifiers.Private:
                    return false;
                case AccessModifiers.ProtectedPrivate:
                case AccessModifiers.ProtectedInternal:
                case AccessModifiers.Protected:
                case AccessModifiers.Public:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(modifiers), modifiers, null);
            }
        }
    }
}