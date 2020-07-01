namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using EnsureThat;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class ChangeCalculator : IChangeCalculator
    {
        private readonly ITypeComparer _comparer;
        private readonly IMatchEvaluator _evaluator;
        private readonly ILogger? _logger;

        public ChangeCalculator(IMatchEvaluator evaluator, ITypeComparer comparer, ILogger? logger)
        {
            Ensure.Any.IsNotNull(evaluator, nameof(evaluator));
            Ensure.Any.IsNotNull(comparer, nameof(comparer));

            _evaluator = evaluator;
            _comparer = comparer;
            _logger = logger;
        }

        public ChangeCalculatorResult CalculateChanges(IEnumerable<ITypeDefinition> oldTypes,
            IEnumerable<ITypeDefinition> newTypes, ComparerOptions options)
        {
            Ensure.Any.IsNotNull(oldTypes, nameof(oldTypes));
            Ensure.Any.IsNotNull(newTypes, nameof(newTypes));

            var result = new ChangeCalculatorResult();

            var changes = CalculateChanges(oldTypes.ToList(), newTypes.ToList(), options);

            foreach (var change in changes)
            {
                result.Add(change);
            }

            _logger?.LogInformation("Calculated overall result as a {0} change.",
                result.ChangeType.ToString().ToLower(CultureInfo.CurrentCulture));

            return result;
        }

        private IEnumerable<ComparisonResult> CalculateChanges(IReadOnlyCollection<ITypeDefinition> oldTypes,
            IReadOnlyCollection<ITypeDefinition> newTypes, ComparerOptions options)
        {
            var oldClasses = oldTypes.OfType<IClassDefinition>();
            var newClasses = newTypes.OfType<IClassDefinition>();

            var classChanges = CalculateChangesByType(oldClasses, newClasses, options);

            var oldInterfaces = oldTypes.OfType<IInterfaceDefinition>();
            var newInterfaces = newTypes.OfType<IInterfaceDefinition>();

            var interfaceChanges = CalculateChangesByType(oldInterfaces, newInterfaces, options);

            return classChanges.Concat(interfaceChanges);
        }

        private IEnumerable<ComparisonResult> CalculateChangesByType(IEnumerable<ITypeDefinition> oldTypes,
            IEnumerable<ITypeDefinition> newTypes, ComparerOptions options)
        {
            var matchingNodes = _evaluator.MatchItems(oldTypes, newTypes, TypeEvaluator);

            // Record any visible types that have been added
            // Types added which are not publicly visible are ignored
            foreach (var memberAdded in matchingNodes.ItemsAdded.Where(x => x.IsVisible))
            {
                var memberRemovedResult = ComparisonResult.ItemAdded(memberAdded);

                yield return memberRemovedResult;
            }

            // Record any visible types that have been removed
            // Types removed which are not publicly visible are ignored
            foreach (var memberRemoved in matchingNodes.ItemsRemoved.Where(x => x.IsVisible))
            {
                var memberRemovedResult = ComparisonResult.ItemRemoved(memberRemoved);

                yield return memberRemovedResult;
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

        private static bool TypeEvaluator(ITypeDefinition oldItem, ITypeDefinition newItem)
        {
            return oldItem.FullName.Equals(newItem.FullName, StringComparison.Ordinal);
        }

        private IEnumerable<ComparisonResult> CompareMatchingItems(ItemMatch<ITypeDefinition> match, ComparerOptions options)
        {
            var results = _comparer.CompareTypes(match, options);

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

            // Recurse down into the child types
            var oldChildTypes = match.OldItem.ChildTypes;
            var newChildTypes = match.NewItem.ChildTypes;

            var childResults = CalculateChangesByType(oldChildTypes, newChildTypes, options);

            foreach (var result in childResults)
            {
                yield return result;
            }
        }
    }
}