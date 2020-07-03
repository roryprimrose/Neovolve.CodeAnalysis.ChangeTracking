namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     The <see cref="ChangeResultAggregator" />
    ///     class is used to collect comparison results and indicate whether further analysis of a node should continue.
    /// </summary>
    public class ChangeResultAggregator
    {
        private readonly List<ComparisonResult> _results = new List<ComparisonResult>();

        /// <summary>
        ///     Adds a result to the aggregator.
        /// </summary>
        /// <param name="result">The result to add.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="result" /> parameter if <c>null</c>.</exception>
        public void AddResult(ComparisonResult result)
        {
            result = result ?? throw new ArgumentNullException(nameof(result));

            _results.Add(result);
        }

        /// <summary>
        ///     Adds a results to the aggregator.
        /// </summary>
        /// <param name="results">The results to add.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="results" /> parameter if <c>null</c>.</exception>
        public void AddResults(IEnumerable<ComparisonResult> results)
        {
            results = results ?? throw new ArgumentNullException(nameof(results));

            _results.AddRange(results);
        }

        /// <summary>
        ///     Merges the specified aggregator into the current aggregator.
        /// </summary>
        /// <param name="aggregator">The aggregator data to merge.</param>
        public void MergeResults(ChangeResultAggregator aggregator)
        {
            aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));

            _results.AddRange(aggregator.Results);
            ExitNodeAnalysis = aggregator.ExitNodeAnalysis;
        }

        /// <summary>
        ///     Gets or sets whether analysis of the current node should continue.
        /// </summary>
        public bool ExitNodeAnalysis { get; set; }

        /// <summary>
        ///     Gets the overall change type.
        /// </summary>
        public SemVerChangeType OverallChangeType
        {
            get
            {
                if (_results.Count == 0)
                {
                    return SemVerChangeType.None;
                }

                return _results.Max(x => x.ChangeType);
            }
        }

        /// <summary>
        ///     Gets the results that have been added to the aggregator.
        /// </summary>
        public IReadOnlyCollection<ComparisonResult> Results => _results;
    }
}