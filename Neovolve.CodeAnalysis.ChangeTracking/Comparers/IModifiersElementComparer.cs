namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IModifiersElementComparer{T}" />
    ///     interface defines the members for comparing <see cref="IModifiersElement{T}"/> items.
    /// </summary>
    /// <typeparam name="T">The type of item to compare.</typeparam>
    public interface IModifiersElementComparer<T> : IItemComparer<IModifiersElement<T>> where T : struct, Enum
    {
    }
}