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

        public override IEnumerable<ComparisonResult> CalculateChanges(IEnumerable<ITypeDefinition> oldItems,
            IEnumerable<ITypeDefinition> newItems, ComparerOptions options)
        {
            var oldTypes = MergePartialTypes(oldItems);
            var newTypes = MergePartialTypes(newItems);

            return base.CalculateChanges(oldTypes, newTypes, options);
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

        private static IEnumerable<ITypeDefinition> MergePartialTypes(IEnumerable<ITypeDefinition> items)
        {
            var types = new Dictionary<string, ITypeDefinition>();

            foreach (var item in items)
            {
                var key = item.GetType().Name + "|" + item.FullName;

                if (types.ContainsKey(key))
                {
                    // Merge this type into the existing type
                    types[key].MergePartialType(item);
                }
                else
                {
                    types.Add(key, item);
                }
            }

            return types.Values;
        }
    }
}