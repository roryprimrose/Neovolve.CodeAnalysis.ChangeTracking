namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class MethodMatchProcessor : MatchProcessor<IMethodDefinition>, IMethodMatchProcessor
    {
        private readonly IMethodComparer _comparer;

        public MethodMatchProcessor(IMethodComparer comparer, IMatchEvaluator<IMethodDefinition> evaluator, ILogger? logger) : base(
            evaluator, logger)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }
        
        protected override IEnumerable<ComparisonResult> EvaluateMatch(ItemMatch<IMethodDefinition> match, ComparerOptions options)
        {
            return _comparer.CompareItems(match, options);
        }

        protected override bool IsVisible(IMethodDefinition item)
        {
            return item.IsVisible;
        }
    }
}