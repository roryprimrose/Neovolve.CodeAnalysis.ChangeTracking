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
        private readonly ITypeMatchProcessor _matchProcessor;

        public ChangeCalculator(ITypeMatchProcessor matchProcessor, ILogger? logger)
        {
            _matchProcessor = matchProcessor ?? throw new ArgumentNullException(nameof(matchProcessor));
            _logger = logger;
        }

        public ChangeCalculatorResult CalculateChanges(IEnumerable<ITypeDefinition> oldTypes,
            IEnumerable<ITypeDefinition> newTypes, ComparerOptions options)
        {
            oldTypes = oldTypes ?? throw new ArgumentNullException(nameof(oldTypes));
            newTypes = newTypes ?? throw new ArgumentNullException(nameof(newTypes));

            var result = new ChangeCalculatorResult();

            var changes = _matchProcessor.CalculateChanges(oldTypes, newTypes, options);

            foreach (var change in changes)
            {
                _logger?.LogInformation(change.Message);

                result.Add(change);
            }

            var changeType = result.ChangeType.ToString().ToLower(CultureInfo.CurrentCulture);

            _logger?.LogInformation("Calculated overall result as a {0} change.", changeType);

            return result;
        }
    }
}