namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using EnsureThat;
    using Microsoft.Extensions.Logging;

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

            var changes = CalculateTypeChanges(oldTypes.FastToList(), newTypes.FastToList(), options);

            foreach (var change in changes)
            {
                result.Add(change);
            }

            _logger?.LogInformation("Calculated overall result as a {0} change.",
                result.ChangeType.ToString().ToLower(CultureInfo.CurrentCulture));

            return result;
        }

        private IEnumerable<ComparisonResult> CalculateTypeChanges(ICollection<ITypeDefinition> oldTypes,
            ICollection<ITypeDefinition> newTypes, ComparerOptions options)
        {
            var oldClasses = oldTypes.OfType<ClassDefinition>();
            var newClasses = newTypes.OfType<ClassDefinition>();

            var classChanges = CalculateTypeChanges(oldClasses, newClasses,
                (oldItem, newItem) => oldItem.FullName.Equals(newItem.FullName, StringComparison.Ordinal), options);

            var oldInterfaces = oldTypes.OfType<InterfaceDefinition>();
            var newInterfaces = newTypes.OfType<InterfaceDefinition>();

            var interfaceChanges = CalculateTypeChanges(oldInterfaces, newInterfaces,
                (oldItem, newItem) => oldItem.FullName.Equals(newItem.FullName, StringComparison.Ordinal), options);

            return classChanges.Concat(interfaceChanges);
        }

        private IEnumerable<ComparisonResult> CalculateTypeChanges(IEnumerable<ITypeDefinition> oldTypes,
            IEnumerable<ITypeDefinition> newTypes, Func<ITypeDefinition, ITypeDefinition, bool> evaluator, ComparerOptions options)
        {
            var matchingNodes = _evaluator.MatchItems(oldTypes, newTypes, evaluator);

            // Record any public members that have been added
            foreach (var memberAdded in matchingNodes.DefinitionsAdded.Where(x => x.IsVisible))
            {
                var memberRemovedResult = ComparisonResult.DefinitionAdded(memberAdded);

                yield return memberRemovedResult;
            }

            // Record any public members that have been removed
            foreach (var memberRemoved in matchingNodes.DefinitionsRemoved.Where(x => x.IsVisible))
            {
                var memberRemovedResult = ComparisonResult.DefinitionRemoved(memberRemoved);

                yield return memberRemovedResult;
            }

            // Check all the matches for a breaking change or feature added
            foreach (var match in matchingNodes.Matches)
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
            }
        }
    }
}