namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     The <see cref="ChangeResultAggregator" />
    ///     class is used to collect comparison results and indicate whether further analysis of a node should continue.
    /// </summary>
    public class ChangeResultAggregator : IChangeResultAggregator
    {
        private readonly List<ComparisonResult> _results = new();

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="result" /> parameter if <c>null</c>.</exception>
        public void AddResult(ComparisonResult result)
        {
            result = result ?? throw new ArgumentNullException(nameof(result));

            _results.Add(result);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="results" /> parameter if <c>null</c>.</exception>
        public void AddResults(IEnumerable<ComparisonResult> results)
        {
            results = results ?? throw new ArgumentNullException(nameof(results));

            _results.AddRange(results);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="aggregator" /> parameter if <c>null</c>.</exception>
        public void MergeResults(IChangeResultAggregator aggregator)
        {
            aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));

            _results.AddRange(aggregator.Results);
            ExitNodeAnalysis = aggregator.ExitNodeAnalysis;
        }

        /// <inheritdoc />
        public bool ExitNodeAnalysis { get; set; }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public IReadOnlyCollection<ComparisonResult> Results => _results;
    }
}