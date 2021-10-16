namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IEvaluator{T}" />
    ///     interface defines the members for identifying matches between old and new items.
    /// </summary>
    /// <typeparam name="T">The type of item to evaluate.</typeparam>
    public interface IEvaluator<T> where T : IItemDefinition
    {
        /// <summary>
        ///     Returns the matching items between old and new items along with added and removed items.
        /// </summary>
        /// <param name="oldItems">The old items to evaluate.</param>
        /// <param name="newItems">The new items to evaluate.</param>
        /// <returns>The matches along with added and removed items.</returns>
        IEvaluationResults<T> FindMatches(IEnumerable<T> oldItems, IEnumerable<T> newItems);
    }
}