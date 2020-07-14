namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;

    [Flags]
    public enum FieldModifiers
    {
        None = 0,
        ReadOnly = 1,
        Static = 2,
        StaticReadOnly = Static | ReadOnly
    }
}