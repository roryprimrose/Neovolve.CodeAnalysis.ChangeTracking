namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IMatchEvaluator
    {
        IMatchResults<T> MatchItems<T>(IEnumerable<T> oldItems, IEnumerable<T> newItems, Func<T, T, bool> evaluator)
            where T : class, IItemDefinition;
    }
}