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

            // Loop in reverse so that the items in the loop can be removed safely by the agent
            for (var index = _oldItems.Count - 1; index >= 0; index--)
            {
                var oldMethod = _oldItems[index];

                // Get the old methods that match the predicate
                var matchingOldMethods = _oldItems.Count(x => condition(oldMethod, x));

                if (matchingOldMethods > 1)
                {
                    // There is more than one old method matching the predicate
                    // We can't match old methods to new methods in this case because there are multiple that could match
                    continue;
                }

                // Get the new methods that also match the predicate
                var matchingMethods = _newItems.Where(x => condition(oldMethod, x)).ToList();

                if (matchingMethods.Count != 1)
                {
                    // There are either no new methods matching the predicate or there are more than one
                    // In either case we can't match the old method to a new method because there are multiple that could match
                    continue;
                }

                var matchingMethod = matchingMethods[0];

                // There is only one old and new method that match the predicate
                // The assumption here is that these two methods are a match
                MatchFound(oldMethod, matchingMethod);
            }
        }

        private void MatchFound(T oldItem, T newItem)
        {
            var match = new ItemMatch<T>(oldItem, newItem);

            _matches.Add(match);
            _oldItems.Remove(oldItem);
            _newItems.Remove(newItem);
        }

        public IReadOnlyCollection<T> NewItems => _newItems.AsReadOnly();
        public IReadOnlyCollection<T> OldItems => _oldItems.AsReadOnly();
        public IMatchResults<T> Results =>
            new MatchResults<T>(_matches, _oldItems, _newItems);
    }
}