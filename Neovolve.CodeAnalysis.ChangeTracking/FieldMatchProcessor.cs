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
            return _comparer.CompareItems(match, options);
        }

        protected override bool IsItemMatch(IFieldDefinition oldItem, IFieldDefinition newItem)
        {
            return oldItem.Name == newItem.Name;
        }

        protected override bool IsVisible(IFieldDefinition item)
        {
            return item.IsVisible;
        }
    }
}