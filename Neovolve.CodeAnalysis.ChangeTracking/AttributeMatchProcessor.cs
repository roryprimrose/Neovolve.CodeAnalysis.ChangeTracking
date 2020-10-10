namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class AttributeMatchProcessor : MatchProcessor<IAttributeDefinition>, IAttributeMatchProcessor
    {
        private readonly IAttributeComparer _comparer;

        public AttributeMatchProcessor(IAttributeComparer comparer, IMatchEvaluator<IAttributeDefinition> evaluator, ILogger? logger) : base(
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

            if (options.CompareAttributes == AttributeCompareOption.Skip)
            {
                // We are not going to evaluate any attributes
                return Array.Empty<ComparisonResult>();
            }

            if (options.CompareAttributes == AttributeCompareOption.All)
            {
                return base.CalculateChanges(oldItems, newItems, options);
            }

            var attributesToMatch = options.AttributeNamesToCompare.ToList();

            // Trim down the items to those where the name matches the compare options
            var oldItemsToCompare = oldItems.Where(x => ShouldCompare(x, attributesToMatch));
            var newItemsToCompare = newItems.Where(x => ShouldCompare(x, attributesToMatch));

            return base.CalculateChanges(oldItemsToCompare, newItemsToCompare, options);
        }

        protected override IEnumerable<ComparisonResult> EvaluateMatch(ItemMatch<IAttributeDefinition> match,
            ComparerOptions options)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            return _comparer.CompareItems(match, options);
        }

        protected override bool IsVisible(IAttributeDefinition item)
        {
            return true;
        }

        private static bool ShouldCompare(IAttributeDefinition item, IEnumerable<Regex> expressions)
        {
            var name = item.GetRawName();

            return expressions.Any(x => x.IsMatch(name));
        }
    }
}