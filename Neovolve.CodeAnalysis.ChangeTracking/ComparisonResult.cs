namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;

    public class ComparisonResult
    {
        private ComparisonResult(SemVerChangeType changeType, IItemDefinition? oldDefinition, IItemDefinition? newDefinition,
            string message)
        {
            ChangeType = changeType;
            OldDefinition = oldDefinition;
            NewDefinition = newDefinition;
            Message = message;
        }

        public static ComparisonResult DefinitionAdded(IElementDefinition newDefinition)
        {
            newDefinition = newDefinition ?? throw new ArgumentNullException(nameof(newDefinition));

            var message = newDefinition + " has been added";

            var changeType = SemVerChangeType.None;

            if (newDefinition.IsVisible)
            {
                changeType = SemVerChangeType.Feature;
            }

            return new ComparisonResult(changeType, null, newDefinition, message);
        }

        public static ComparisonResult DefinitionChanged<T>(SemVerChangeType changeType, ItemMatch<T> match,
            string message) where T : class, IItemDefinition
        {
            changeType = changeType == SemVerChangeType.None
                ? throw new ArgumentException("The changeType cannot be None to indicate a change on the member.",
                    nameof(changeType))
                : changeType;
            match = match ?? throw new ArgumentNullException(nameof(match));
            message = string.IsNullOrWhiteSpace(message) ? throw new ArgumentException(nameof(message)) : message;

            return new ComparisonResult(changeType, match.OldItem, match.NewItem, message);
        }

        public static ComparisonResult DefinitionRemoved(IElementDefinition oldDefinition)
        {
            oldDefinition = oldDefinition ?? throw new ArgumentNullException(nameof(oldDefinition));

            var message = oldDefinition + " has been removed";

            var changeType = SemVerChangeType.None;

            if (oldDefinition.IsVisible)
            {
                changeType = SemVerChangeType.Breaking;
            }

            return new ComparisonResult(changeType, oldDefinition, null, message);
        }

        public static ComparisonResult NoChange<T>(ItemMatch<T> match) where T : class, IItemDefinition
        {
            match = match ?? throw new ArgumentNullException(nameof(match));

            var message = "No change on " + match.OldItem;

            return new ComparisonResult(SemVerChangeType.None, match.OldItem, match.NewItem, message);
        }

        public SemVerChangeType ChangeType { get; }

        public string Message { get; }

        public IItemDefinition? NewDefinition { get; }

        public IItemDefinition? OldDefinition { get; }
    }
}