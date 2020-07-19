namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;

    public static class IsVisibleExtensions
    {
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