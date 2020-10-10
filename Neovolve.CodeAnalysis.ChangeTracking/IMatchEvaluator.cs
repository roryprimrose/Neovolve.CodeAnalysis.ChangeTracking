namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IMatchEvaluator<T>
            where T : IItemDefinition
    {
        IMatchResults<T> MatchItems(IEnumerable<T> oldItems, IEnumerable<T> newItems);
    }
}