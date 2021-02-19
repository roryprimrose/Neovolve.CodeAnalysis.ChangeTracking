namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IMatchEvaluator<T> where T : IItemDefinition
    {
        IMatchResults<T> FindMatches(IEnumerable<T> oldItems, IEnumerable<T> newItems);
    }
}