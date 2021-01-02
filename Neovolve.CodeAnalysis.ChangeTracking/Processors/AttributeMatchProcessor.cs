namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
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

        protected override bool IsItemMatch(IAttributeDefinition oldItem, IAttributeDefinition newItem)
        {
            oldItem = oldItem ?? throw new ArgumentNullException(nameof(oldItem));
            newItem = newItem ?? throw new ArgumentNullException(nameof(newItem));

            var oldName = StripAttributeSuffix(oldItem.Name);
            var newName = StripAttributeSuffix(newItem.Name);

            // NOTE: This is not able to adequately handle multiple attribute definitions
            // Unfortunately there is no accurate way to match up different usages of the same attribute type when the argument list may have been altered
            return oldName == newName;
        }

        protected override bool IsVisible(IAttributeDefinition item)
        {
            return true;
        }

        private static bool ShouldCompare(IAttributeDefinition item, IEnumerable<Regex> expressions)
        {
            var name = StripAttributeSuffix(item.Name);

            return expressions.Any(x => x.IsMatch(name));
        }

        private static string StripAttributeSuffix(string name)
        {
            // This assumes that the expressions in ComparerOptions do not handle the Attribute suffix that is not required by the compiler
            if (name.EndsWith("Attribute", StringComparison.OrdinalIgnoreCase))
            {
                return name.Substring(0, name.Length - 9);
            }

            return name;
        }
    }
}