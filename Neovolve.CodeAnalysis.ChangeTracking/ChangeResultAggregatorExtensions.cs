namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class ChangeResultAggregatorExtensions
    {
        public static void AddElementAddedResult<T>(this IChangeResultAggregator aggregator,
            SemVerChangeType changeType, T element,
            IMessageFormatter messageFormatter)
            where T : IItemDefinition
        {
            aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
            element = element ?? throw new ArgumentNullException(nameof(element));
            messageFormatter = messageFormatter ?? throw new ArgumentNullException(nameof(messageFormatter));

            var args = new FormatArguments("has been added", null, null);

            var message = messageFormatter.FormatItem(element, ItemFormatType.ItemAdded, args);

            var result = new ComparisonResult(changeType, null, element, message);

            aggregator.AddResult(result);
        }

        public static void AddElementChangedResult<T>(this IChangeResultAggregator aggregator,
            SemVerChangeType changeType,
            ItemMatch<T> match,
            IMessageFormatter messageFormatter,
            FormatArguments arguments)
            where T : IItemDefinition
        {
            aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
            match = match ?? throw new ArgumentNullException(nameof(match));
            messageFormatter = messageFormatter ?? throw new ArgumentNullException(nameof(messageFormatter));
            arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));

            var message = messageFormatter.FormatMatch(match, ItemFormatType.ItemChanged, arguments);

            var result = new ComparisonResult(
                changeType,
                match.OldItem, match.NewItem,
                message);

            aggregator.AddResult(result);
        }

        public static void AddElementRemovedResult<T>(this IChangeResultAggregator aggregator,
            SemVerChangeType changeType, T element,
            IMessageFormatter messageFormatter)
            where T : IItemDefinition
        {
            aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
            element = element ?? throw new ArgumentNullException(nameof(element));
            messageFormatter = messageFormatter ?? throw new ArgumentNullException(nameof(messageFormatter));

            var args = new FormatArguments("has been removed", null, null);

            var message = messageFormatter.FormatItem(element, ItemFormatType.ItemRemoved, args);

            var result = new ComparisonResult(changeType, element, null, message);

            aggregator.AddResult(result);
        }
    }
}