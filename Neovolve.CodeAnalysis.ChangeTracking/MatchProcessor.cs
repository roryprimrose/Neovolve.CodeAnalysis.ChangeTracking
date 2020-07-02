namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using System.Linq;
    using EnsureThat;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public abstract class MatchProcessor<T> : IMatchProcessor<T> where T : class, IItemDefinition
    {
        private readonly IMatchEvaluator _evaluator;
        private readonly ILogger? _logger;

        protected MatchProcessor(IMatchEvaluator evaluator, ILogger? logger)
        {
            Ensure.Any.IsNotNull(evaluator, nameof(evaluator));

            _evaluator = evaluator;
            _logger = logger;
        }

        public IEnumerable<ComparisonResult> CalculateChanges(IEnumerable<T> oldItems,
            IEnumerable<T> newItems, ComparerOptions options)
        {
            var matchingNodes = _evaluator.MatchItems(oldItems, newItems, IsItemMatch);

            // Record any visible types that have been added
            // Types added which are not publicly visible are ignored
            foreach (var memberAdded in matchingNodes.ItemsAdded.Where(IsVisible))
            {
                var itemAdded = ComparisonResult.ItemAdded(memberAdded);

                yield return itemAdded;
            }

            // Record any visible types that have been removed
            // Types removed which are not publicly visible are ignored
            foreach (var memberRemoved in matchingNodes.ItemsRemoved.Where(IsVisible))
            {
                var itemRemoved = ComparisonResult.ItemRemoved(memberRemoved);

                yield return itemRemoved;
            }

            // Check all the matches for a breaking change or feature added
            foreach (var match in matchingNodes.MatchingItems)
            {
                var comparisonResults = CompareMatchingItems(match, options);

                foreach (var result in comparisonResults)
                {
                    yield return result;
                }
            }
        }

        protected abstract IEnumerable<ComparisonResult> EvaluateMatch(ItemMatch<T> match, ComparerOptions options);

        protected abstract bool IsItemMatch(T oldItem, T newItem);
        protected abstract bool IsVisible(T item);

        private IEnumerable<ComparisonResult> CompareMatchingItems(ItemMatch<T> match,
            ComparerOptions options)
        {
            var results = EvaluateMatch(match, options);

            foreach (var result in results)
            {
                if (result.ChangeType == SemVerChangeType.None)
                {
                    _logger?.LogDebug(result.Message);

                    // Don't add comparison results to the outcome where it looks like there is no change
                    continue;
                }

                _logger?.LogInformation(result.Message);

                yield return result;
            }
        }
    }
}