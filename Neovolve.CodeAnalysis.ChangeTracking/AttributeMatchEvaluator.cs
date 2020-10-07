namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class AttributeMatchEvaluator : MatchEvaluator<IAttributeDefinition>
    {
        public override IMatchResults<IAttributeDefinition> MatchItems(
            IEnumerable<IAttributeDefinition> oldItems,
            IEnumerable<IAttributeDefinition> newItems)
        {
            oldItems = oldItems ?? throw new ArgumentNullException(nameof(oldItems));
            newItems = newItems ?? throw new ArgumentNullException(nameof(newItems));

            var results = new MatchResults<IAttributeDefinition>(oldItems, newItems);

            return FindMatches(results, (x, y) => x.Name == y.Name);
        }
    }
}