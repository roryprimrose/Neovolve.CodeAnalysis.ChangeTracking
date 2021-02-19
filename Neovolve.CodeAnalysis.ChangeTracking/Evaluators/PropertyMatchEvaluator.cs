namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyMatchEvaluator : MatchEvaluator<IPropertyDefinition>, IPropertyMatchEvaluator
    {
        public override IMatchResults<IPropertyDefinition> FindMatches(
            IEnumerable<IPropertyDefinition> oldItems,
            IEnumerable<IPropertyDefinition> newItems)
        {
            oldItems = oldItems ?? throw new ArgumentNullException(nameof(oldItems));
            newItems = newItems ?? throw new ArgumentNullException(nameof(newItems));

            var results = new MatchResults<IPropertyDefinition>(oldItems, newItems);

            return FindMatches(results, (x, y) => x.Name == y.Name);
        }
    }
}