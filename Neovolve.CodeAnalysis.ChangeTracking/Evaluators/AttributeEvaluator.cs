namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class AttributeEvaluator : Evaluator<IAttributeDefinition>, IAttributeEvaluator
    {
        public override IMatchResults<IAttributeDefinition> FindMatches(
            IEnumerable<IAttributeDefinition> oldItems,
            IEnumerable<IAttributeDefinition> newItems)
        {
            oldItems = oldItems ?? throw new ArgumentNullException(nameof(oldItems));
            newItems = newItems ?? throw new ArgumentNullException(nameof(newItems));

            var results = new MatchResults<IAttributeDefinition>(oldItems, newItems);

            return FindMatches(results, (x, y) => x.GetRawName() == y.GetRawName());
        }
    }
}