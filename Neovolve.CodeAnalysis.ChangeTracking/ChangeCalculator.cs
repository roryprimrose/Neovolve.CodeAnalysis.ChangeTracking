namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class ChangeCalculator : IChangeCalculator
    {
        private readonly ILogger? _logger;
        private readonly IBaseTypeMatchProcessor _matchProcessor;

        public ChangeCalculator(IBaseTypeMatchProcessor matchProcessor, ILogger? logger)
        {
            _matchProcessor = matchProcessor ?? throw new ArgumentNullException(nameof(matchProcessor));
            _logger = logger;
        }

        public ChangeCalculatorResult CalculateChanges(IEnumerable<IBaseTypeDefinition> oldTypes,
            IEnumerable<IBaseTypeDefinition> newTypes, ComparerOptions options)
        {
            oldTypes = oldTypes ?? throw new ArgumentNullException(nameof(oldTypes));
            newTypes = newTypes ?? throw new ArgumentNullException(nameof(newTypes));

            var result = new ChangeCalculatorResult();

            var changes = _matchProcessor.CalculateChanges(oldTypes, newTypes, options);

            foreach (var change in changes)
            {
                if (change.ChangeType == SemVerChangeType.None)
                {
                    continue;
                }

                _logger?.LogInformation($@"{change.ChangeType}: {change.Message}");

                result.Add(change);
            }

            var changeType = result.ChangeType.ToString().ToLower(CultureInfo.CurrentCulture);

            _logger?.LogInformation("Calculated overall result as a {0} change.", changeType);

            return result;
        }
    }
}