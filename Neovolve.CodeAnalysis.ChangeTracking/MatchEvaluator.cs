namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class MatchEvaluator : IMatchEvaluator
    {
        public IMatchResults<T> MatchItems<T>(IEnumerable<T> oldItems, IEnumerable<T> newItems, Func<T, T, bool> evaluator) where T : IItemDefinition
        {
            if (oldItems == null)
            {
                throw new ArgumentNullException(nameof(oldItems));
            }

            if (newItems == null)
            {
                throw new ArgumentNullException(nameof(newItems));
            }

            var matches = new List<ItemMatch<T>>();

            var oldDefinitions = oldItems.ToList();
            var newDefinitions = newItems.ToList();

            // Loop in reverse so that we can remove matched members as we go
            // Removing matched members as we find them means that we have less iterations of the inner loop for each subsequent old member
            // The set of old members and new members can also then be reported as not matches once all matches are removed
            // A for loop is required here because removing items would break a foreach iterator
            for (var oldIndex = oldDefinitions.Count - 1; oldIndex >= 0; oldIndex--)
            {
                var oldItem = oldDefinitions[oldIndex];

                for (var newIndex = newDefinitions.Count - 1; newIndex >= 0; newIndex--)
                {
                    var newItem = newDefinitions[newIndex];

                    if (evaluator(oldItem, newItem))
                    {
                        var match = new ItemMatch<T>(oldItem, newItem);

                        // Track the match
                        matches.Add(match);

                        // Remove the indices
                        newDefinitions.RemoveAt(oldIndex);
                        oldDefinitions.RemoveAt(newIndex);

                        break;
                    }
                }
            }

            return new MatchResults<T>(matches, oldDefinitions, newDefinitions);
        }
    }
}