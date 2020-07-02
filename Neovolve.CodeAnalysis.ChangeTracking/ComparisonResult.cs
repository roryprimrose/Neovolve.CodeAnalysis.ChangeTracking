namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class ComparisonResult
    {
        private ComparisonResult(SemVerChangeType changeType, IItemDefinition? oldItem, IItemDefinition? newItem,
            string message)
        {
            ChangeType = changeType;
            OldItem = oldItem;
            NewItem = newItem;
            Message = message;
        }

        public static ComparisonResult ItemAdded(IItemDefinition newItem)
        {
            newItem = newItem ?? throw new ArgumentNullException(nameof(newItem));

            var isVisible = true;

            if (newItem is IElementDefinition element)
            {
                isVisible = element.IsVisible;
            }

            var message = newItem.Description + " has been added";

            var changeType = SemVerChangeType.None;

            if (isVisible)
            {
                changeType = SemVerChangeType.Feature;
            }

            return new ComparisonResult(changeType, null, newItem, message);
        }

        public static ComparisonResult ItemChanged<T>(SemVerChangeType changeType, ItemMatch<T> match,
            string message) where T : IItemDefinition
        {
            changeType = changeType == SemVerChangeType.None
                ? throw new ArgumentException("The changeType cannot be None to indicate a change on the member.",
                    nameof(changeType))
                : changeType;
            match = match ?? throw new ArgumentNullException(nameof(match));
            message = string.IsNullOrWhiteSpace(message) ? throw new ArgumentException(nameof(message)) : message;

            return new ComparisonResult(changeType, match.OldItem, match.NewItem, message);
        }

        public static ComparisonResult ItemRemoved(IItemDefinition oldItem)
        {
            oldItem = oldItem ?? throw new ArgumentNullException(nameof(oldItem));

            var isVisible = true;

            if (oldItem is IElementDefinition element)
            {
                isVisible = element.IsVisible;
            }

            var message = oldItem.Description + " has been removed";

            var changeType = SemVerChangeType.None;

            if (isVisible)
            {
                changeType = SemVerChangeType.Breaking;
            }

            return new ComparisonResult(changeType, oldItem, null, message);
        }

        public static ComparisonResult NoChange<T>(ItemMatch<T> match) where T : IItemDefinition
        {
            match = match ?? throw new ArgumentNullException(nameof(match));

            var message = "No change on " + match.OldItem.Description;

            return new ComparisonResult(SemVerChangeType.None, match.OldItem, match.NewItem, message);
        }

        public SemVerChangeType ChangeType { get; }

        public string Message { get; }

        public IItemDefinition? NewItem { get; }

        public IItemDefinition? OldItem { get; }
    }
}