namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class BaseTypeMatchProcessor : ElementMatchProcessor<IBaseTypeDefinition>, IBaseTypeMatchProcessor
    {
        public BaseTypeMatchProcessor(
            IBaseTypeEvaluator evaluator,
            IBaseTypeComparer comparer,
            ILogger? logger) : base(evaluator, comparer, logger)
        {
        }

        public override IEnumerable<ComparisonResult> CalculateChanges(IEnumerable<IBaseTypeDefinition> oldItems,
            IEnumerable<IBaseTypeDefinition> newItems, ComparerOptions options)
        {
            var oldTypes = MergePartialTypes(oldItems);
            var newTypes = MergePartialTypes(newItems);

            return base.CalculateChanges(oldTypes, newTypes, options);
        }

        protected override IEnumerable<ComparisonResult> EvaluateMatch(
            ItemMatch<IBaseTypeDefinition> match,
            ComparerOptions options)
        {
            var results = base.EvaluateMatch(match, options);

            foreach (var result in results)
            {
                yield return result;
            }

            if (match.OldItem is ITypeDefinition oldTypeDefinition
                && match.NewItem is ITypeDefinition newTypeDefinition)
            {
                // Recurse down into the child types
                var oldChildTypes = oldTypeDefinition.ChildTypes;
                var newChildTypes = newTypeDefinition.ChildTypes;

                var childResults = CalculateChanges(oldChildTypes, newChildTypes, options);

                foreach (var result in childResults)
                {
                    yield return result;
                }
            }
        }

        private static IEnumerable<IBaseTypeDefinition> MergePartialTypes(IEnumerable<IBaseTypeDefinition> items)
        {
            var otherTypes = new List<IBaseTypeDefinition>();
            var types = new Dictionary<string, ITypeDefinition>();

            foreach (var item in items)
            {
                if (item is not ITypeDefinition typeDefinition)
                {
                    otherTypes.Add(item);

                    continue;
                }

                var key = item.GetType().Name + "|" + item.FullName;

                if (types.ContainsKey(key))
                {
                    // Merge this type into the existing type
                    types[key].MergePartialType(typeDefinition);
                }
                else
                {
                    types.Add(key, typeDefinition);
                }
            }

            return otherTypes.Union(types.Values);
        }
    }
}