namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class FieldMatchProcessor : MatchProcessor<IFieldDefinition>, IFieldMatchProcessor
    {
        private readonly IFieldComparer _comparer;

        public FieldMatchProcessor(IFieldComparer comparer, IMatchEvaluator evaluator, ILogger? logger) : base(
            evaluator, logger)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        protected override IEnumerable<ComparisonResult> EvaluateMatch(ItemMatch<IFieldDefinition> match,
            ComparerOptions options)
        {
            var results = _comparer.CompareItems(match, options);

            foreach (var result in results)
            {
                yield return result;
            }
        }

        protected override bool IsItemMatch(IFieldDefinition oldItem, IFieldDefinition newItem)
        {
            return oldItem.FullName == newItem.FullName;
        }

        protected override bool IsVisible(IFieldDefinition item)
        {
            return item.IsVisible;
        }
    }
}