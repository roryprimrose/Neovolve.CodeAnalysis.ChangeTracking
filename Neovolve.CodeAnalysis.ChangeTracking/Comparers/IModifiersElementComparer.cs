namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IModifiersElementComparer<T> : IItemComparer<IModifiersElement<T>> where T : struct, Enum
    {
    }
}