namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public abstract class MatchEvaluator<T> : IMatchEvaluator<T> where T : IItemDefinition
    {
        protected virtual IMatchResults<T> FindMatches(IMatchResults<T> results, Func<T, T, bool> evaluator)
        {
            results = results ?? throw new ArgumentNullException(nameof(results));

            var matches = new List<ItemMatch<T>>(results.MatchingItems);

            var oldDefinitions = results.ItemsRemoved.FastToList();
            var newDefinitions = results.ItemsAdded.FastToList();

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
                        newDefinitions.RemoveAt(newIndex);
                        oldDefinitions.RemoveAt(oldIndex);

                        break;
                    }
                }
            }

            return new MatchResults<T>(matches, oldDefinitions, newDefinitions);
        }

        public abstract IMatchResults<T> MatchItems(IEnumerable<T> oldItems, IEnumerable<T> newItems);
    }
}