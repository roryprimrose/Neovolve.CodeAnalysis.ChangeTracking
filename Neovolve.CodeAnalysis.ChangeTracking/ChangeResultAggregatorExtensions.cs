namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class ChangeResultAggregatorExtensions
    {
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

            var message = messageFormatter.FormatItemChangedMessage(match, arguments);

            var result = new ComparisonResult(
                changeType,
                match.OldItem, match.NewItem,
                message);

            aggregator.AddResult(result);
        }
    }
}