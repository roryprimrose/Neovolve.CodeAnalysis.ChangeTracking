namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using EnsureThat;

    public class MatchResults<T> where T : class, IItemDefinition
    {
        public MatchResults(IEnumerable<ItemMatch<T>> matchingItems, IEnumerable<T> itemsRemoved,
            IEnumerable<T> itemsAdded)
        {
            Ensure.Any.IsNotNull(matchingItems, nameof(matchingItems));
            Ensure.Any.IsNotNull(itemsRemoved, nameof(itemsRemoved));
            Ensure.Any.IsNotNull(itemsAdded, nameof(itemsAdded));

            Matches = new ReadOnlyCollection<ItemMatch<T>>(matchingItems.FastToList());
            DefinitionsRemoved = new ReadOnlyCollection<T>(itemsRemoved.FastToList());
            DefinitionsAdded = new ReadOnlyCollection<T>(itemsAdded.FastToList());
        }

        public IReadOnlyCollection<T> DefinitionsAdded { get; }

        public IReadOnlyCollection<T> DefinitionsRemoved { get; }
        public IReadOnlyCollection<ItemMatch<T>> Matches { get; }
    }
}