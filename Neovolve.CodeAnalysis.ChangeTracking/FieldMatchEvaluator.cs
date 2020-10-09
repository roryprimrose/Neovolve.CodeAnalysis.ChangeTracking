namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class FieldMatchEvaluator : MatchEvaluator<IFieldDefinition>
    {
        public override IMatchResults<IFieldDefinition> MatchItems(
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