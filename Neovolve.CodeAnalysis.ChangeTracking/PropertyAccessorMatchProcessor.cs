namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyAccessorMatchProcessor : MatchProcessor<IPropertyAccessorDefinition>, IPropertyAccessorMatchProcessor
    {
        private readonly IPropertyAccessorComparer _comparer;

        public PropertyAccessorMatchProcessor(IPropertyAccessorComparer comparer, IMatchEvaluator<IPropertyAccessorDefinition> evaluator, ILogger? logger) : base(
            evaluator, logger)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        protected override IEnumerable<ComparisonResult> EvaluateMatch(ItemMatch<IPropertyAccessorDefinition> match,
            ComparerOptions options)
        {
            return _comparer.CompareItems(match, options);
        }

        protected override bool IsVisible(IPropertyAccessorDefinition item)
        {
            return item.IsVisible;
        }
    }
}