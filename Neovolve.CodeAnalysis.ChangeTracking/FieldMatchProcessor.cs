namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class FieldMatchProcessor : MatchProcessor<IFieldDefinition>, IFieldMatchProcessor
    {
        private readonly IFieldComparer _comparer;

        public FieldMatchProcessor(IFieldComparer comparer, IMatchEvaluator<IFieldDefinition> evaluator, ILogger? logger) : base(
            evaluator, logger)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        protected override IEnumerable<ComparisonResult> EvaluateMatch(ItemMatch<IFieldDefinition> match,
            ComparerOptions options)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            return _comparer.CompareItems(match, options);
        }

        protected override bool IsItemMatch(IFieldDefinition oldItem, IFieldDefinition newItem)
        {
            oldItem = oldItem ?? throw new ArgumentNullException(nameof(oldItem));
            newItem = newItem ?? throw new ArgumentNullException(nameof(newItem));

            return oldItem.Name == newItem.Name;
        }

        protected override bool IsVisible(IFieldDefinition item)
        {
            item = item ?? throw new ArgumentNullException(nameof(item));

            return item.IsVisible;
        }
    }
}