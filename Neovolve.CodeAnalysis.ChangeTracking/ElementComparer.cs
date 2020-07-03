﻿namespace Neovolve.CodeAnalysis.ChangeTracking
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

            if (match.OldItem.IsVisible == false
                && match.NewItem.IsVisible == false)
            {
                // It doesn't matter if there is a change to the type because it isn't visible anyway
                yield return ComparisonResult.NoChange(match);

                // No need to check for further changes
                yield break;
            }

            foreach (var comparisonResult in EvaluateAccessModifiers(match))
            {
                yield return comparisonResult;
            }

            foreach (var comparisonResult in EvaluateMatch(match, options))
            {
                yield return comparisonResult;
            }

            foreach (var comparisonResult in EvaluateAttributeChanges(match, options))
            {
                yield return comparisonResult;
            }
        }

        protected abstract IEnumerable<ComparisonResult> EvaluateMatch(ItemMatch<T> match, ComparerOptions options);

        private static string DetermineScopeMessage(string scope)
        {
            if (string.IsNullOrWhiteSpace(scope))
            {
                return "(implicit) private";
            }

            return scope;
        }

        private static IEnumerable<ComparisonResult> EvaluateAccessModifiers(ItemMatch<T> match)
        {
            var oldScope = DetermineScopeMessage(match.OldItem.AccessModifiers);
            var newScope = DetermineScopeMessage(match.NewItem.AccessModifiers);

            if (match.OldItem.IsVisible
                && match.NewItem.IsVisible == false)
            {
                // The member was visible but isn't now, breaking change
                var message = $"{match.NewItem.Description} changed scope from {oldScope} to {newScope}";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    message);
            }
            else if (match.OldItem.IsVisible == false
                     && match.NewItem.IsVisible)
            {
                // The member return type may have changed, but the member is only now becoming public
                // This is a feature because the public API didn't break even if the return type has changed
                var message = $"{match.NewItem.Description} changed scope from {oldScope} to {newScope}";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match,
                    message);
            }
        }

        private IEnumerable<ComparisonResult> EvaluateAttributeChanges(ItemMatch<T> match, ComparerOptions options)
        {
            var attributeResults = _attributeProcessor.CalculateChanges(match.OldItem.Attributes,
                match.NewItem.Attributes, options);

            foreach (var result in attributeResults)
            {
                yield return result;
            }
        }
    }
}