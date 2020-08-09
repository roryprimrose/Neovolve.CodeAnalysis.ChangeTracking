namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;

    [Flags]
    public enum StructModifiers
    {
        None = 0,
        ReadOnly = 1,
        Partial = 2,
        ReadOnlyPartial = ReadOnly | Partial
    }
}