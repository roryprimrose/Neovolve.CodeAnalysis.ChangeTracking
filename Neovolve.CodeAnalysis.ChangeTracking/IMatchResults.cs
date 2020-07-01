namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IMatchResults{T}" />
    ///     interface defines the members that describe how items have changed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMatchResults<T> where T : class, IItemDefinition
    {
        /// <summary>
        ///     Returns the items that have been added.
        /// </summary>
        IReadOnlyCollection<T> ItemsAdded { get; }

        /// <summary>
        ///     Returns the items that have been removed.
        /// </summary>
        IReadOnlyCollection<T> ItemsRemoved { get; }

        /// <summary>
        ///     Returns the items that match between old and new items.
        /// </summary>
        IReadOnlyCollection<ItemMatch<T>> MatchingItems { get; }
    }
}