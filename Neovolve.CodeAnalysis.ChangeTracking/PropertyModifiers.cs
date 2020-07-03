namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;

    [Flags]
    public enum PropertyModifiers
    {
        None = 0,
        Abstract = 1,
        New = 2,
        Override = 4,
        Sealed = 8,
        Static = 16,
        Virtual = 32,
        NewAbstract = New | Abstract,
        NewStatic = New | Static,
        NewVirtual = New | Virtual,
        SealedVirtual = Sealed | Override
    }
}