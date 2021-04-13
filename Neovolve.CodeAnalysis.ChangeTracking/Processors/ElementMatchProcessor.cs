namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using System;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public abstract class ElementMatchProcessor<T> : MatchProcessor<T>, IElementMatchProcessor<T>
        where T : IElementDefinition
    {
        protected ElementMatchProcessor(IEvaluator<T> evaluator, IItemComparer<T> comparer, ILogger? logger) : base(evaluator, comparer, logger)
        {
        }

        protected override bool IsVisible(T item)
        {
            item = item ?? throw new ArgumentNullException(nameof(item));

            return item.IsVisible;
        }
    }
}