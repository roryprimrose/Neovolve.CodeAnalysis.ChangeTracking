namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;

    [Flags]
    public enum MethodModifiers
    {
        None = 0,
        Abstract = 1,
        New = 2,
        Override = 4,
        Sealed = 8,
        Static = 16,
        Virtual = 32,
        Async = 64,
        AsyncAbstract = Async | Abstract,
        AsyncNew = Async | New,
        AsyncOverride = Async | Override,
        AsyncSealed = Async | Sealed,
        AsyncStatic = Async | Static,
        AsyncVirtual = Async | Virtual,
        AbstractOverride = Abstract | Override,
        NewAbstract = New | Abstract,
        NewAbstractVirtual = New | Abstract | Virtual,
        NewStatic = New | Static,
        NewVirtual = New | Virtual,
        SealedOverride = Sealed | Override,
        AsyncAbstractOverride = Async | Abstract | Override,
        AsyncNewAbstract = Async | New | Abstract,
        AsyncNewAbstractVirtual = Async | New | Abstract | Virtual,
        AsyncNewStatic = Async | New | Static,
        AsyncNewVirtual = Async | New | Virtual,
        AsyncSealedOverride = Async | Sealed | Override
    }
}