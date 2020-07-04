namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;

    public static class AccessModifierExtensions
    {
        public static bool IsVisible(this AccessModifier modifier)
        {
            switch (modifier)
            {
                case AccessModifier.None:
                    throw new InvalidOperationException("The modifier must be specified to determine the visibility");
                case AccessModifier.Internal:
                case AccessModifier.Private:
                case AccessModifier.ProtectedInternal:
                case AccessModifier.ProtectedPrivate:
                    return false;
                case AccessModifier.Protected:
                case AccessModifier.Public:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(modifier), modifier, null);
            }
        }
    }
}