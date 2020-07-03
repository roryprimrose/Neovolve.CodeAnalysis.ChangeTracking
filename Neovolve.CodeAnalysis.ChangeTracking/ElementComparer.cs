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

        public virtual IEnumerable<ComparisonResult> CompareItems(ItemMatch<T> match, ComparerOptions options)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var aggregator = new ChangeResultAggregator();

            if (match.OldItem.IsVisible == false
                && match.NewItem.IsVisible == false)
            {
                // It doesn't matter if there is a change to the type because it isn't visible anyway
                // No need to check for further changes
                return aggregator.Results;
            }

            RunComparisonStep(EvaluateAccessModifiers, match, options, aggregator);
            RunComparisonStep(EvaluateMatch, match, options, aggregator);
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
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (aggregator == null)
            {
                throw new ArgumentNullException(nameof(aggregator));
            }

            if (aggregator.ExitNodeAnalysis)
            {
                return;
            }

            step(match, options, aggregator);
        }

        private static string DetermineScopeMessage(string scope)
        {
            if (string.IsNullOrWhiteSpace(scope))
            {
                return "(implicit) private";
            }

            return scope;
        }

        private static void EvaluateAccessModifiers(
            ItemMatch<T> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            var oldScope = DetermineScopeMessage(match.OldItem.AccessModifiers);
            var newScope = DetermineScopeMessage(match.NewItem.AccessModifiers);

            if (match.OldItem.IsVisible
                && match.NewItem.IsVisible == false)
            {
                // The member was visible but isn't now, breaking change
                var message = $"{match.NewItem.Description} changed scope from {oldScope} to {newScope}";

                var result = ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);

                aggregator.AddResult(result);
                aggregator.ExitNodeAnalysis = true;
            }
            else if (match.OldItem.IsVisible == false
                     && match.NewItem.IsVisible)
            {
                // The member return type may have changed, but the member is only now becoming public
                // This is a feature because the public API didn't break even if the return type has changed
                var message = $"{match.NewItem.Description} changed scope from {oldScope} to {newScope}";

                var result = ComparisonResult.ItemChanged(SemVerChangeType.Feature, match, message);

                aggregator.AddResult(result);
                aggregator.ExitNodeAnalysis = true;
            }
        }

        private void EvaluateAttributeChanges(
            ItemMatch<T> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            var attributeResults = _attributeProcessor.CalculateChanges(
                match.OldItem.Attributes,
                match.NewItem.Attributes,
                options);

            foreach (var result in attributeResults)
            {
                aggregator.AddResult(result);
            }
        }
    }
}