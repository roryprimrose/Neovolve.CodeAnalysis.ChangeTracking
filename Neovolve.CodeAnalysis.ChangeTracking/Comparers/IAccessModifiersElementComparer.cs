namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IAccessModifiersElementComparer{T}" />
    ///     interface defines the members for comparing <see cref="IAccessModifiersElement{T}"/> items.
    /// </summary>
    /// <typeparam name="T">The type of item to compare.</typeparam>
    public interface IAccessModifiersElementComparer<T> : IItemComparer<IAccessModifiersElement<T>>
        where T : struct, Enum
    {
    }
}