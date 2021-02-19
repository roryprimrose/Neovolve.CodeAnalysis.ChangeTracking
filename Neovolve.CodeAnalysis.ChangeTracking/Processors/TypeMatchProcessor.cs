namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TypeMatchProcessor : ElementMatchProcessor<ITypeDefinition>, ITypeMatchProcessor
    {
        public TypeMatchProcessor(
            ITypeEvaluator evaluator,
            ITypeComparer comparer,
            ILogger? logger) : base(evaluator, comparer, logger)
        {
        }

        protected override IEnumerable<ComparisonResult> EvaluateMatch(
            ItemMatch<ITypeDefinition> match,
            ComparerOptions options)
        {
            var results = base.EvaluateMatch(match, options);

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