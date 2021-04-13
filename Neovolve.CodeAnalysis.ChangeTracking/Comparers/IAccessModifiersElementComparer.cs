namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IAccessModifiersElementComparer<T> : IItemComparer<IAccessModifiersElement<T>>
        where T : struct, Enum
    {
    }
}