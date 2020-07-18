namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class ItemMatch<T> where T : IItemDefinition
    {
        public ItemMatch(T oldItem, T newItem)
        {
            OldItem = oldItem ?? throw new ArgumentNullException(nameof(oldItem));
            NewItem = newItem ?? throw new ArgumentNullException(nameof(newItem));
        }

        public T NewItem { get; }

        public T OldItem { get; }
    }
}