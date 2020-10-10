namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyAccessorMatchEvaluator : MatchEvaluator<IPropertyAccessorDefinition>
    {
        public override IMatchResults<IPropertyAccessorDefinition> MatchItems(
            IEnumerable<IPropertyAccessorDefinition> oldItems,
            IEnumerable<IPropertyAccessorDefinition> newItems)
        {
            oldItems = oldItems ?? throw new ArgumentNullException(nameof(oldItems));
            newItems = newItems ?? throw new ArgumentNullException(nameof(newItems));

            var results = new MatchResults<IPropertyAccessorDefinition>(oldItems, newItems);

            return FindMatches(results, (x, y) => x.Name == y.Name);
        }
    }
}