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

        protected override IEnumerable<ComparisonResult> EvaluateMatch(ItemMatch<IAttributeDefinition> match,
            ComparerOptions options)
        {
            return _comparer.CompareTypes(match, options);
        }

        protected override bool IsItemMatch(IAttributeDefinition oldItem, IAttributeDefinition newItem)
        {
            return oldItem.Name == newItem.Name;
        }

        protected override bool IsVisible(IAttributeDefinition item)
        {
            return true;
        }
    }
}