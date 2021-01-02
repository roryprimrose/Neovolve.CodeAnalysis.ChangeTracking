namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TypeMatchProcessor : MatchProcessor<ITypeDefinition>, ITypeMatchProcessor
    {
        private readonly ITypeComparer _comparer;

        public TypeMatchProcessor(ITypeComparer comparer, IMatchEvaluator evaluator, ILogger? logger) : base(evaluator, logger)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        protected override bool IsItemMatch(ITypeDefinition oldItem, ITypeDefinition newItem)
        {
            return oldItem.IsMatch(newItem);
        }

        protected override bool IsVisible(ITypeDefinition item)
        {
            return item.IsVisible;
        }

        protected override IEnumerable<ComparisonResult> EvaluateMatch(ItemMatch<ITypeDefinition> match, ComparerOptions options)
        {
            var results = _comparer.CompareItems(match, options);

            foreach (var result in results)
            {
                yield return result;
            }

            // Recurse down into the child types
            var oldChildTypes = match.OldItem.ChildTypes;
            var newChildTypes = match.NewItem.ChildTypes;

            var childResults = CalculateChanges(oldChildTypes, newChildTypes, options);

            foreach (var result in childResults)
            {
                yield return result;
            }
        }
    }
}