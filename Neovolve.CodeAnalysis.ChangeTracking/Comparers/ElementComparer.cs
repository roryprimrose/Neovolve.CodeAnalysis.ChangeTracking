namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public abstract class ElementComparer<T> : IElementComparer<T> where T : IElementDefinition
    {
        private readonly IAttributeMatchProcessor _attributeProcessor;

        protected ElementComparer(IAttributeMatchProcessor attributeProcessor)
        {
            _attributeProcessor = attributeProcessor ?? throw new ArgumentNullException(nameof(attributeProcessor));
        }

        public IEnumerable<ComparisonResult> CompareMatch(ItemMatch<T> match, ComparerOptions options)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            var aggregator = new ChangeResultAggregator();

            // We don't want this member to be virtual because we need to ensure that the evaluation of IsVisible occurs before any other evaluation in derived classes
            // If this method was virtual, a derived class could run evaluations that push results onto the aggregator before the logic here has a chance to ignore changes
            // on elements based on their visibility

            // Neither element is visible so don't bother evaluating changes
            if (match.OldItem.IsVisible == false
                && match.NewItem.IsVisible == false)
            {
                // It doesn't matter if there is a change to the element because it isn't visible anyway
                // No need to check for further changes
                return aggregator.Results;
            }

            // If the member was previously hidden but is now visible then we need to record this as a feature change even if other comparison checks look like a breaking change
            // For example, changing the return type is still a feature change if the element is moving from private to public
            // If the member was previously visible but is now hidden then this is a breaking change and further comparison checks are not required
            if (match.OldItem.IsVisible != match.NewItem.IsVisible)
            {
                var oldAccessModifiers = match.OldItem.GetDeclaredAccessModifiers();
                var newAccessModifiers = match.NewItem.GetDeclaredAccessModifiers();

                var formatArguments = new FormatArguments(
                    "access modifiers have changed from {OldValue} to {NewValue}",
                    oldAccessModifiers,
                    newAccessModifiers);

                var changeType = match.OldItem.IsVisible ? SemVerChangeType.Breaking : SemVerChangeType.Feature;

                aggregator.AddElementChangedResult(changeType, match, options.MessageFormatter, formatArguments);

                return aggregator.Results;
            }

            EvaluateElementMatch(match, options, aggregator);

            return aggregator.Results;
        }

        protected virtual void EvaluateChildElementChanges(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
        }

        protected virtual void EvaluateElementMatch(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            RunComparisonStep(EvaluateTypeDefinitionChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateModifierChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateSignatureChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateChildElementChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateAttributeChanges, match, options, aggregator);
        }

        protected virtual void EvaluateModifierChanges(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
        }

        protected virtual void EvaluateSignatureChanges(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
        }

        /// <summary>
        ///     Evaluate changes to the type definition such as changes to namespace or the type itself (class -> interface).
        /// </summary>
        /// <param name="match">The item match to evaluate.</param>
        /// <param name="options">The comparer options.</param>
        /// <param name="aggregator">The results aggregator.</param>
        protected virtual void EvaluateTypeDefinitionChanges(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
        }

        protected void RunComparisonStep(
            Action<ItemMatch<T>, ComparerOptions, IChangeResultAggregator> step,
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            RunComparisonStep(step, match, options, aggregator, false);
        }

        protected void RunComparisonStep(
            Action<ItemMatch<T>, ComparerOptions, IChangeResultAggregator> step,
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator,
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
            IChangeResultAggregator aggregator)
        {
            var results = _attributeProcessor.CalculateChanges(
                match.OldItem.Attributes,
                match.NewItem.Attributes,
                options);

            aggregator.AddResults(results);
        }
    }
}