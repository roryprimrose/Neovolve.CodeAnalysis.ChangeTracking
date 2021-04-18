namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;

    [Flags]
    public enum InterfaceModifiers
    {
        None = 0,
        New = 1,
        Partial = 2,
        NewPartial = New | Partial
    }
}