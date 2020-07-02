namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using EnsureThat;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public abstract class ElementComparer<T> : IElementComparer<T> where T : IElementDefinition
    {
        public virtual IEnumerable<ComparisonResult> CompareTypes(ItemMatch<T> match, ComparerOptions options)
        {
            Ensure.Any.IsNotNull(match, nameof(match));
            Ensure.Any.IsNotNull(options, nameof(options));

            if (string.Equals(match.OldItem.FullName, match.NewItem.FullName, StringComparison.Ordinal) == false)
            {
                throw new InvalidOperationException(
                    "The two items cannot be compared because they have different Name values.");
            }

            if (match.OldItem.IsVisible == false
                && match.NewItem.IsVisible == false)
            {
                // It doesn't matter if there is a change to the type because it isn't visible anyway
                yield return ComparisonResult.NoChange(match);

                // No need to check for further changes
                yield break;
            }

            foreach (var comparisonResult in EvaluateScopeChanges(match))
            {
                yield return comparisonResult;
            }

            foreach (var comparisonResult in EvaluateMatch(match, options))
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

        private static IEnumerable<ComparisonResult> EvaluateScopeChanges(ItemMatch<T> match)
        {
            var oldScope = DetermineScopeMessage(match.OldItem.Scope);
            var newScope = DetermineScopeMessage(match.NewItem.Scope);

            if (match.OldItem.IsVisible
                && match.NewItem.IsVisible == false)
            {
                // The member was visible but isn't now, breaking change
                var message = $"{match.OldItem.Description} changed scope from {oldScope} to {newScope}";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    message);
            }
            else if (match.OldItem.IsVisible == false
                     && match.NewItem.IsVisible)
            {
                // The member return type may have changed, but the member is only now becoming public
                // This is a feature because the public API didn't break even if the return type has changed
                var message = $"{match.OldItem.Description} changed scope from {oldScope} to {newScope}";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match,
                    message);
            }
        }
    }
}