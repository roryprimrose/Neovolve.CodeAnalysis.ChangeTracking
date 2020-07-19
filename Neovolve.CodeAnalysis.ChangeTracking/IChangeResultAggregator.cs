namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;

    public interface IChangeResultAggregator
    {
        /// <summary>
        ///     Adds a result to the aggregator.
        /// </summary>
        /// <param name="result">The result to add.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="result" /> parameter if <c>null</c>.</exception>
        void AddResult(ComparisonResult result);

        /// <summary>
        ///     Adds a results to the aggregator.
        /// </summary>
        /// <param name="results">The results to add.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="results" /> parameter if <c>null</c>.</exception>
        void AddResults(IEnumerable<ComparisonResult> results);

        /// <summary>
        ///     Merges the specified aggregator into the current aggregator.
        /// </summary>
        /// <param name="aggregator">The aggregator data to merge.</param>
        void MergeResults(IChangeResultAggregator aggregator);

        /// <summary>
        ///     Gets or sets whether analysis of the current node should continue.
        /// </summary>
        bool ExitNodeAnalysis { get; set; }

        /// <summary>
        ///     Gets the overall change type.
        /// </summary>
        SemVerChangeType OverallChangeType { get; }

        /// <summary>
        ///     Gets the results that have been added to the aggregator.
        /// </summary>
        IReadOnlyCollection<ComparisonResult> Results { get; }
    }
}