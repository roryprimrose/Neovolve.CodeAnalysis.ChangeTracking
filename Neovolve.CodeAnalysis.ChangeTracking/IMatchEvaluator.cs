namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;

    public interface IMatchEvaluator
    {
        MatchResults<T> MatchItems<T>(IEnumerable<T> oldItems, IEnumerable<T> newItems, Func<T, T, bool> evaluator)
            where T : class, IItemDefinition;
    }
}