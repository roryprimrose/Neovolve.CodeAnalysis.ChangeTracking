namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public abstract class Evaluator<T> : IEvaluator<T> where T : IItemDefinition
    {
        public virtual IMatchResults<T> FindMatches(IEnumerable<T> oldItems, IEnumerable<T> newItems)
        {
            oldItems = oldItems ?? throw new ArgumentNullException(nameof(oldItems));
            newItems = newItems ?? throw new ArgumentNullException(nameof(newItems));

            var agent = new MatchAgent<T>(oldItems, newItems);

            FindMatches(agent);

            return agent.Results;
        }

        protected abstract void FindMatches(IMatchAgent<T> agent);
    }
}