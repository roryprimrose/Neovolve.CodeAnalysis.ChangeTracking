namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IItemComparer{T}" />
    ///     interface defines the members for comparing items.
    /// </summary>
    /// <typeparam name="T">The type of item to compare.</typeparam>
    public interface IItemComparer<T> where T : IItemDefinition
    {
        /// <summary>
        ///     Compares the specified match and returns the comparison results.
        /// </summary>
        /// <param name="match">The match to evaluate.</param>
        /// <param name="options">The comparison options.</param>
        /// <returns>The changes identified when comparing the item match.</returns>
        IEnumerable<ComparisonResult> CompareItems(ItemMatch<T> match, ComparerOptions options);
    }
}