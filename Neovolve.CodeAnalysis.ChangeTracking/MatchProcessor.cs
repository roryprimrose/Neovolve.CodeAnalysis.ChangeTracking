namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public abstract class MatchProcessor<T> : IMatchProcessor<T> where T : IItemDefinition
    {
        private readonly IItemComparer<T> _comparer;
        private readonly IMatchEvaluator<T> _evaluator;
        private readonly ILogger? _logger;

        protected MatchProcessor(IMatchEvaluator<T> evaluator, IItemComparer<T> comparer, ILogger? logger)
        {
            _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _logger = logger;
        }

        public virtual IEnumerable<ComparisonResult> CalculateChanges(
            IEnumerable<T> oldItems,
            IEnumerable<T> newItems,
            ComparerOptions options)
        {
            oldItems = oldItems ?? throw new ArgumentNullException(nameof(oldItems));
            newItems = newItems ?? throw new ArgumentNullException(nameof(newItems));
            options = options ?? throw new ArgumentNullException(nameof(options));

            IMatchResults<T> matchingNodes = _evaluator.MatchItems(oldItems, newItems);

            // Record any visible types that have been added
            // Types added which are not publicly visible are ignored
            foreach (var memberAdded in matchingNodes.ItemsAdded.Where(IsVisible))
            {
                var isVisible = true;
                var name = memberAdded.Name;

                if (memberAdded is IElementDefinition element)
                {
                    isVisible = element.IsVisible;
                    name = element.FullName;
                }

                var changeType = SemVerChangeType.None;

                if (isVisible)
                {
                    changeType = SemVerChangeType.Feature;
                }

                var args = new FormatArguments("{DefinitionType} {Identifier} has been added", name, null, null);

                var message = options.MessageFormatter.FormatItemAddedMessage(memberAdded, args);

                var result = new ComparisonResult(changeType, null, memberAdded, message);

                yield return result;
            }

            // Record any visible types that have been removed
            // Types removed which are not publicly visible are ignored
            foreach (var memberRemoved in matchingNodes.ItemsRemoved.Where(IsVisible))
            {
                var isVisible = true;
                var name = memberRemoved.Name;

                if (memberRemoved is IElementDefinition element)
                {
                    isVisible = element.IsVisible;
                    name = element.FullName;
                }

                var changeType = SemVerChangeType.None;

                if (isVisible)
                {
                    changeType = SemVerChangeType.Breaking;
                }

                var args = new FormatArguments("{DefinitionType} {Identifier} has been removed", name, null, null);

                var message = options.MessageFormatter.FormatItemRemovedMessage(memberRemoved, args);

                var result = new ComparisonResult(changeType, memberRemoved, null, message);

                yield return result;
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

        protected virtual IEnumerable<ComparisonResult> EvaluateMatch(ItemMatch<T> match, ComparerOptions options)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            return _comparer.CompareItems(match, options);
        }

        protected abstract bool IsVisible(T item);

        private IEnumerable<ComparisonResult> CompareMatchingItems(ItemMatch<T> match, ComparerOptions options)
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

                yield return result;
            }
        }
    }
}