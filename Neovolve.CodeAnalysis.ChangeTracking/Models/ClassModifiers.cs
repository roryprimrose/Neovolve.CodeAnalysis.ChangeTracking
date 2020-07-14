namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;

    [Flags]
    public enum ClassModifiers
    {
        None = 0,
        Abstract = 1,
        Partial = 2,
        Sealed = 4,
        Static = 8,
        AbstractPartial = Abstract | Partial,
        SealedPartial = Sealed | Partial,
        StaticPartial = Static | Partial
    }
}