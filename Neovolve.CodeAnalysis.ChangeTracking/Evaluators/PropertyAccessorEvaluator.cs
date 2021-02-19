namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyAccessorEvaluator : Evaluator<IPropertyAccessorDefinition>, IPropertyAccessorEvaluator
    {
        public override IMatchResults<IPropertyAccessorDefinition> FindMatches(
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