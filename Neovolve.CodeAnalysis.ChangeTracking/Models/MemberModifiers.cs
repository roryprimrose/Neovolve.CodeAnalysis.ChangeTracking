namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;

    [Flags]
    public enum MemberModifiers
    {
        None = 0,
        Abstract = 1,
        New = 2,
        Override = 4,
        Sealed = 8,
        Static = 16,
        Virtual = 32,
        AbstractOverride = Abstract | Override,
        NewAbstract = New | Abstract,
        NewAbstractVirtual = New | Abstract | Virtual,
        NewStatic = New | Static,
        NewVirtual = New | Virtual,
        SealedOverride = Sealed | Override
    }
}