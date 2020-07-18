namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class AttributeMatchProcessor : MatchProcessor<IAttributeDefinition>, IAttributeMatchProcessor
    {
        private readonly IAttributeComparer _comparer;

        public AttributeMatchProcessor(IAttributeComparer comparer, IMatchEvaluator evaluator, ILogger? logger) : base(
            evaluator, logger)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        public override IEnumerable<ComparisonResult> CalculateChanges(IEnumerable<IAttributeDefinition> oldItems,
            IEnumerable<IAttributeDefinition> newItems, ComparerOptions options)
        {
            oldItems = oldItems ?? throw new ArgumentNullException(nameof(oldItems));
            newItems = newItems ?? throw new ArgumentNullException(nameof(newItems));
            options = options ?? throw new ArgumentNullException(nameof(options));

            if (options.SkipAttributes)
            {
                // We are not going to evaluate any attributes
                return Array.Empty<ComparisonResult>();
            }

            return base.CalculateChanges(oldItems, newItems, options);
        }

        protected override IEnumerable<ComparisonResult> EvaluateMatch(ItemMatch<IAttributeDefinition> match,
            ComparerOptions options)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            return _comparer.CompareTypes(match, options);
        }

        protected override bool IsItemMatch(IAttributeDefinition oldItem, IAttributeDefinition newItem)
        {
            oldItem = oldItem ?? throw new ArgumentNullException(nameof(oldItem));
            newItem = newItem ?? throw new ArgumentNullException(nameof(newItem));

            // NOTE: This is not able to adequately handle multiple attribute definitions
            // Unfortunately there is no accurate way to match up different usages of the same attribute type when the argument list may have been altered
            return oldItem.Name == newItem.Name;
        }

        protected override bool IsVisible(IAttributeDefinition item)
        {
            return true;
        }
    }
}