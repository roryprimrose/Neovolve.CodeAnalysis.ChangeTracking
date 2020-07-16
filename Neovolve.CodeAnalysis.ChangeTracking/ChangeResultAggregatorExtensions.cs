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
            where T : IElementDefinition
        {
            aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
            messageFormatter = messageFormatter ?? throw new ArgumentNullException(nameof(messageFormatter));
            arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));

            var definition = match.OldItem ?? match.NewItem;

            var message = messageFormatter.FormatMessage(definition, arguments);

            var result = new ComparisonResult(
                changeType,
                match.OldItem, match.NewItem,
                message);

            aggregator.AddResult(result);
        }
    }
}