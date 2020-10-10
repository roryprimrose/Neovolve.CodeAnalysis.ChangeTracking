namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyMatchProcessor : MatchProcessor<IPropertyDefinition>, IPropertyMatchProcessor
    {
        private readonly IPropertyComparer _comparer;

        public PropertyMatchProcessor(IPropertyComparer comparer, IMatchEvaluator<IPropertyDefinition> evaluator, ILogger? logger) : base(
            evaluator, logger)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        protected override IEnumerable<ComparisonResult> EvaluateMatch(ItemMatch<IPropertyDefinition> match,
            ComparerOptions options)
        {
            return _comparer.CompareItems(match, options);
        }

        protected override bool IsVisible(IPropertyDefinition item)
        {
            return item.IsVisible;
        }
    }
}