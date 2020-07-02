namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
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
            // Types are the same if they have the same name with the same number of generic type parameters
            // Check the number of generic type parameters first because if the number is different then it doesn't matter about the name
            // If it is a generic type then we need to parse the type parameters out to validate the name
            if (oldItem.GenericTypeParameters.Count != newItem.GenericTypeParameters.Count)
            {
                return false;
            }

            if (oldItem.GenericTypeParameters.Count == 0)
            {
                return oldItem.FullName == newItem.FullName;
            }

            // Both the types are generic types
            var oldName = oldItem.GetFullNameWithoutGenericTypes();
            var newName = newItem.GetFullNameWithoutGenericTypes();

            return oldName == newName;
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