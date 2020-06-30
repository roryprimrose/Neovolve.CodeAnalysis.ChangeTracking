namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using EnsureThat;

    public class ItemMatch<T> where T : class, IItemDefinition
    {
        public ItemMatch(T oldItem, T newItem)
        {
            Ensure.Any.IsNotNull(oldItem, nameof(oldItem));
            Ensure.Any.IsNotNull(newItem, nameof(newItem));

            OldItem = oldItem;
            NewItem = newItem;
        }

        public T NewItem { get; }

        public T OldItem { get; }
    }
}