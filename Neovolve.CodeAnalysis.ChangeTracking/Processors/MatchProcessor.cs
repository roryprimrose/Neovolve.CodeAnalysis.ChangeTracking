﻿namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public abstract class MatchProcessor<T> : IMatchProcessor<T> where T : IItemDefinition
    {
        private readonly IItemComparer<T> _comparer;
        private readonly IEvaluator<T> _evaluator;
        private readonly ILogger? _logger;

        protected MatchProcessor(IEvaluator<T> evaluator, IItemComparer<T> comparer, ILogger? logger)
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

            IEvaluationResults<T> matchingNodes = _evaluator.FindMatches(oldItems, newItems);

            // Record any visible items that have been added
            // Items added which are not publicly visible are ignored
            foreach (var memberAdded in matchingNodes.ItemsAdded)
            {
                var isVisible = IsVisible(memberAdded);

                if (isVisible)
                {
                    var args = new FormatArguments("has been added", null, null);

                    var message = options.MessageFormatter.FormatItem(memberAdded, ItemFormatType.ItemAdded, args);

                    var changeType = GetItemAddedChangeType(memberAdded);

                    var result = new ComparisonResult(changeType, null, memberAdded, message);

                    yield return result;
                }
                else if (_logger != null
                         && _logger.IsEnabled(LogLevel.Debug))
                {
                    var args = new FormatArguments("has been added but is not visible", null, null);

                    var message = options.MessageFormatter.FormatItem(memberAdded, ItemFormatType.ItemAdded, args);

                    _logger.LogDebug(message);
                }
            }

            // Record any visible items that have been removed
            // Items removed which are not publicly visible are ignored
            foreach (var memberRemoved in matchingNodes.ItemsRemoved)
            {
                var isVisible = IsVisible(memberRemoved);

                if (isVisible)
                {
                    var args = new FormatArguments("has been removed", null, null);

                    var message = options.MessageFormatter.FormatItem(memberRemoved, ItemFormatType.ItemRemoved, args);

                    var result = new ComparisonResult(SemVerChangeType.Breaking, memberRemoved, null, message);

                    yield return result;
                }
                else if (_logger != null
                         && _logger.IsEnabled(LogLevel.Debug))
                {
                    var args = new FormatArguments("has been removed but is not visible", null, null);

                    var message = options.MessageFormatter.FormatItem(memberRemoved, ItemFormatType.ItemRemoved, args);

                    _logger.LogDebug(message);
                }
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

            return _comparer.CompareMatch(match, options);
        }

        protected virtual SemVerChangeType GetItemAddedChangeType(T memberAdded)
        {
            return SemVerChangeType.Feature;
        }

        protected abstract bool IsVisible(T item);

        private IEnumerable<ComparisonResult> CompareMatchingItems(ItemMatch<T> match, ComparerOptions options)
        {
            var results = EvaluateMatch(match, options);

            foreach (var result in results)
            {
                _logger?.LogDebug(result.Message);

                if (result.ChangeType == SemVerChangeType.None)
                {
                    // Don't add comparison results to the outcome where it looks like there is no change
                    continue;
                }

                yield return result;
            }
        }
    }
}