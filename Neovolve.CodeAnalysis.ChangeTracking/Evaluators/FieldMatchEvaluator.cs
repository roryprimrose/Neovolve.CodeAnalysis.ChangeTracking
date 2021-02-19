namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class FieldMatchEvaluator : MatchEvaluator<IFieldDefinition>, IFieldMatchEvaluator
    {
        public override IMatchResults<IFieldDefinition> FindMatches(
            IEnumerable<IFieldDefinition> oldItems,
            IEnumerable<IFieldDefinition> newItems)
        {
            oldItems = oldItems ?? throw new ArgumentNullException(nameof(oldItems));
            newItems = newItems ?? throw new ArgumentNullException(nameof(newItems));

            var results = new MatchResults<IFieldDefinition>(oldItems, newItems);

            return FindMatches(results, (x, y) => x.Name == y.Name);
        }
    }
}