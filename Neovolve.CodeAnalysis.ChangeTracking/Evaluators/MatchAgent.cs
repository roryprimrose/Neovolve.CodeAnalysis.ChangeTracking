namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class MatchAgent<T> : IMatchAgent<T> where T : IItemDefinition
    {
        private readonly List<ItemMatch<T>> _matches;
        private readonly List<T> _newItems;
        private readonly List<T> _oldItems;

        public MatchAgent(IEnumerable<T> oldItems, IEnumerable<T> newItems)
        {
            oldItems = oldItems ?? throw new ArgumentNullException(nameof(oldItems));
            newItems = newItems ?? throw new ArgumentNullException(nameof(newItems));

            _oldItems = oldItems.ToList();
            _newItems = newItems.ToList();
            _matches = new List<ItemMatch<T>>();
        }

        public void MatchOn(Func<T, T, bool> condition)
        {
            condition = condition ?? throw new ArgumentNullException(nameof(condition));

            // Exit early if it is not possible to make any further matches
            if (_oldItems.Count == 0)
            {
                return;
            }

            if (_newItems.Count == 0)
            {
                return;
            }

            // Loop in reverse so that the items in the loop can be removed safely by the agent
            for (var index = _oldItems.Count - 1; index >= 0; index--)
            {
                var oldItem = _oldItems[index];

                // Get the old items that match the predicate
                var matchingOldItems = _oldItems.Count(x => condition(oldItem, x));

                if (matchingOldItems > 1)
                {
                    // There is more than one old item matching the predicate
                    // We can't match old items to new items in this case because there are multiple that could match
                    continue;
                }

                // Get the new items that also match the predicate
                var matchingItems = _newItems.Where(x => condition(oldItem, x)).ToList();

                if (matchingItems.Count != 1)
                {
                    // There are either no new items matching the predicate or there are more than one
                    // In either case we can't match the old item to a new item because there are multiple that could match
                    continue;
                }

                var newItem = matchingItems[0];

                // There is only one old and new item that match the predicate
                // The assumption here is that these two items are a match
                MatchFound(oldItem, newItem);
            }
        }

        private void MatchFound(T oldItem, T newItem)
        {
            var match = new ItemMatch<T>(oldItem, newItem);

            _matches.Add(match);
            _oldItems.Remove(oldItem);
            _newItems.Remove(newItem);
        }

        public IEvaluationResults<T> Results =>
            new EvaluationResults<T>(_matches, _oldItems, _newItems);
    }
}