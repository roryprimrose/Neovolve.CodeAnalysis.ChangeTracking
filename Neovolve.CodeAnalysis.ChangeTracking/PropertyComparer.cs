namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using EnsureThat;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyComparer : IPropertyComparer
    {
        public IEnumerable<ComparisonResult> CompareTypes(ItemMatch<IPropertyDefinition> match, ComparerOptions options)
        {
            Ensure.Any.IsNotNull(match, nameof(match));
            Ensure.Any.IsNotNull(options, nameof(options));

            if (string.Equals(match.OldItem.FullName, match.NewItem.FullName, StringComparison.Ordinal) == false)
            {
                throw new InvalidOperationException(
                    "The two members cannot be compared because they have different Name values.");
            }

            if (match.OldItem.IsVisible == false
                && match.NewItem.IsVisible == false)
            {
                // It doesn't matter if there is a change to the type because it isn't visible anyway
                yield return ComparisonResult.NoChange(match);
            }

            foreach (var comparisonResult in EvaluateScopeChanges(match))
            {
                yield return comparisonResult;
            }

            foreach (var comparisonResult in EvaluateModifierChanges(match))
            {
                yield return comparisonResult;
            }

            // Calculate breaking changes
            if (match.OldItem.CanRead
                && match.NewItem.CanRead == false)
            {
                var message = match.OldItem.Description + " removed the get accessor";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);
            }
            else if (match.OldItem.CanRead == false
                     && match.NewItem.CanRead)
            {
                var message = match.OldItem.Description + " added a get accessor";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match, message);
            }
            
            if (match.OldItem.CanWrite
                     && match.NewItem.CanWrite == false)
            {
                var message = match.OldItem.Description + " removed the set accessor";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);
            }
            else if (match.OldItem.CanWrite == false
                     && match.NewItem.CanWrite)
            {
                var message = match.OldItem.Description + " added a set accessor";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match, message);
            }
        }

        private static string DetermineScopeMessage(string scope)
        {
            if (string.IsNullOrWhiteSpace(scope))
            {
                return "(implicit) private";
            }

            return scope;
        }

        private static IEnumerable<ComparisonResult> EvaluateModifierChanges(ItemMatch<IPropertyDefinition> match)
        {
            //var oldClass = match.OldItem as IClassDefinition;

            //if (oldClass == null)
            //{
            //    yield break;
            //}

            //var newClass = (IClassDefinition)match.NewItem;

            //if (oldClass.IsAbstract
            //    && newClass.IsAbstract == false)
            //{
            //    yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match,
            //        $"{oldClass.Description} has removed the abstract keyword");
            //}
            //else if (oldClass.IsAbstract == false
            //         && newClass.IsAbstract)
            //{
            //    yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
            //        $"{oldClass.Description} has added the abstract keyword");
            //}

            //if (oldClass.IsSealed
            //    && newClass.IsSealed == false)
            //{
            //    yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match,
            //        $"{oldClass.Description} has removed the sealed keyword");
            //}
            //else if (oldClass.IsSealed == false
            //         && newClass.IsSealed)
            //{
            //    yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
            //        $"{oldClass.Description} has added the sealed keyword");
            //}

            //if (oldClass.IsStatic
            //    && newClass.IsStatic == false)
            //{
            //    yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
            //        $"{oldClass.Description} has removed the static keyword");
            //}
            //else if (oldClass.IsStatic == false
            //         && newClass.IsStatic)
            //{
            //    yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
            //        $"{oldClass.Description} has added the static keyword");
            //}
            yield break;
        }

        private static IEnumerable<ComparisonResult> EvaluateScopeChanges(ItemMatch<IPropertyDefinition> match)
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