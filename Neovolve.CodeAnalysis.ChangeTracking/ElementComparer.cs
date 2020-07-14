namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public abstract class ElementComparer<T> : IElementComparer<T> where T : IElementDefinition
    {
        private readonly IAttributeMatchProcessor _attributeProcessor;

        protected ElementComparer(IAttributeMatchProcessor attributeProcessor)
        {
            _attributeProcessor = attributeProcessor ?? throw new ArgumentNullException(nameof(attributeProcessor));
        }

        public IEnumerable<ComparisonResult> CompareItems(ItemMatch<T> match, ComparerOptions options)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            var aggregator = new ChangeResultAggregator();

            // We don't want this member to be virtual because we need to ensure that the evaluation of IsVisible occurs before any other evaluation in derived classes
            // If this method was virtual, a derived class could run evaluations that push results onto the aggregator before the logic here has a chance to ignore changes
            // on elements that are not visible
            if (match.OldItem.IsVisible == false
                && match.NewItem.IsVisible == false)
            {
                // It doesn't matter if there is a change to the type because it isn't visible anyway
                // No need to check for further changes
                return aggregator.Results;
            }

            RunComparisonStep(EvaluateMatch, match, options, aggregator, true);
            RunComparisonStep(EvaluateAttributeChanges, match, options, aggregator);

            return aggregator.Results;
        }

        protected abstract void EvaluateMatch(
            ItemMatch<T> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator);

        protected void RunComparisonStep(
            Action<ItemMatch<T>, ComparerOptions, ChangeResultAggregator> step,
            ItemMatch<T> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            RunComparisonStep(step, match, options, aggregator, false);
        }

        protected void RunComparisonStep(
            Action<ItemMatch<T>, ComparerOptions, ChangeResultAggregator> step,
            ItemMatch<T> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator,
            bool exitOnBreakingChange)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));
            aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));

            if (aggregator.ExitNodeAnalysis)
            {
                return;
            }

            step(match, options, aggregator);

            if (exitOnBreakingChange == false)
            {
                // Don't bother calculating the biggest outcome if we are not going to set the exit flag anyway
                return;
            }

            if (aggregator.OverallChangeType == SemVerChangeType.Breaking)
            {
                aggregator.ExitNodeAnalysis = true;
            }
        }

        private void EvaluateAttributeChanges(
            ItemMatch<T> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            var results = _attributeProcessor.CalculateChanges(
                match.OldItem.Attributes,
                match.NewItem.Attributes,
                options);

            aggregator.AddResults(results);
        }
    }
}