namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using EnsureThat;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class ChangeCalculator : IChangeCalculator
    {
        private readonly ILogger? _logger;
        private readonly IMatchProcessor<ITypeDefinition> _matchProcessor;

        public ChangeCalculator(IMatchProcessor<ITypeDefinition> matchProcessor, ILogger? logger)
        {
            _matchProcessor = matchProcessor ?? throw new ArgumentNullException(nameof(matchProcessor));
            _logger = logger;
        }

        public ChangeCalculatorResult CalculateChanges(IEnumerable<ITypeDefinition> oldTypes,
            IEnumerable<ITypeDefinition> newTypes, ComparerOptions options)
        {
            Ensure.Any.IsNotNull(oldTypes, nameof(oldTypes));
            Ensure.Any.IsNotNull(newTypes, nameof(newTypes));

            var result = new ChangeCalculatorResult();

            var changes = _matchProcessor.CalculateChanges(oldTypes, newTypes, options);

            foreach (var change in changes)
            {
                result.Add(change);
            }

            var changeType = result.ChangeType.ToString().ToLower(CultureInfo.CurrentCulture);

            _logger?.LogInformation("Calculated overall result as a {0} change.", changeType);

            return result;
        }
    }
}