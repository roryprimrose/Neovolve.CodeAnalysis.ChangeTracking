namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class ItemMatch<T> where T : IItemDefinition
    {
        public ItemMatch(T oldItem, T newItem)
        {
            OldItem = oldItem;
            NewItem = newItem;
        }

        public T NewItem { get; }

        public T OldItem { get; }
    }
}