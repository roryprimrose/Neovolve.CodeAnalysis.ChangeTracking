namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyComparer : MemberComparer<IPropertyDefinition>, IPropertyComparer
    {
        public PropertyComparer(IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
        }

        protected override IEnumerable<ComparisonResult> EvaluateMatch(ItemMatch<IPropertyDefinition> match,
            ComparerOptions options)
        {
            // Include results from the base class
            foreach (var result in base.EvaluateMatch(match, options))
            {
                yield return result;
            }

            foreach (var comparisonResult in EvaluateModifierChanges(match))
            {
                yield return comparisonResult;
            }

            // Calculate breaking changes
            if (match.OldItem.CanRead
                && match.NewItem.CanRead == false)
            {
                var message = match.NewItem.Description + " removed the get accessor";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);
            }
            else if (match.OldItem.CanRead == false
                     && match.NewItem.CanRead)
            {
                var message = match.NewItem.Description + " added a get accessor";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match, message);
            }

            if (match.OldItem.CanWrite
                && match.NewItem.CanWrite == false)
            {
                var message = match.NewItem.Description + " removed the set accessor";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);
            }
            else if (match.OldItem.CanWrite == false
                     && match.NewItem.CanWrite)
            {
                var message = match.NewItem.Description + " added a set accessor";

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
            var oldItem = match.OldItem;
            var newItem = match.NewItem;

            if (oldItem.IsStatic
                && newItem.IsStatic == false)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{newItem.Description} has removed the static keyword");
            }
            else if (oldItem.IsStatic == false
                     && newItem.IsStatic)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{newItem.Description} has added the static keyword");
            }

            if (oldItem.IsSealed
                && newItem.IsSealed == false
                && newItem.IsOverride == false 
                && newItem.IsVirtual == false)
            {
                // We couldn't override the member and still can't
                // No-op
            }
            else if (oldItem.IsSealed
                     && newItem.IsSealed == false)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match,
                    $"{newItem.Description} has removed the sealed keyword");
            }
            else if (oldItem.IsSealed == false
                     && oldItem.IsOverride == false
                     && newItem.IsOverride
                     && newItem.IsSealed)
            {
                // We couldn't override the member and still can't
                // No-op
            }
            else if (oldItem.IsSealed == false
                     && (oldItem.IsVirtual || oldItem.IsOverride)
                     && newItem.IsSealed)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{newItem.Description} has added the sealed keyword");
            }

            if (oldItem.IsAbstract
                && newItem.IsAbstract == false
                && newItem.IsSealed
                && newItem.IsOverride)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{newItem.Description} has replaced the abstract keyword with sealed override");
            }
            else if (oldItem.IsAbstract
                     && newItem.IsAbstract == false
                     && newItem.IsOverride)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match,
                    $"{newItem.Description} has replaced the abstract keyword with override");
            }
            else if (oldItem.IsAbstract
                     && newItem.IsAbstract == false
                     && newItem.IsVirtual)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match,
                    $"{newItem.Description} has replaced the abstract keyword with virtual");
            }
            else if (oldItem.IsAbstract
                     && newItem.IsAbstract == false)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{newItem.Description} has removed the abstract keyword");
            }
            else if (oldItem.IsAbstract == false
                     && newItem.IsAbstract)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{newItem.Description} has added the abstract keyword");
            }

            if (oldItem.IsVirtual
                && newItem.IsVirtual == false
                && newItem.IsSealed
                && newItem.IsOverride)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{newItem.Description} has replaced the virtual keyword with sealed override");
            }
            else if (oldItem.IsVirtual
                     && newItem.IsVirtual == false
                     && newItem.IsOverride)
            {
                // The property can still be overridden and callers don't need to change
                // No-op
            }
            else if (oldItem.IsVirtual
                     && newItem.IsVirtual == false
                     && newItem.IsOverride == false)
            {
                // The property can no longer be overridden
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{newItem.Description} has removed the virtual keyword");
            }
            else if (oldItem.IsVirtual == false
                     && oldItem.IsOverride == false
                     && newItem.IsVirtual)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match,
                    $"{newItem.Description} has added the virtual keyword");
            }

            if (oldItem.IsOverride
                && oldItem.IsSealed
                && newItem.IsOverride == false)
            {
                // The property still can't be overridden and callers don't need to change
                // No-op
            }
            else if (oldItem.IsOverride
                     && newItem.IsOverride == false
                     && newItem.IsVirtual == false)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{newItem.Description} has removed the override keyword");
            }
            else if (oldItem.IsOverride
                     && newItem.IsOverride == false
                     && newItem.IsVirtual)
            {
                // The property can still be overridden and callers don't need to change
                // No-op
            }
            else if (oldItem.IsOverride == false
                     && oldItem.IsVirtual == false
                     && newItem.IsSealed == false
                     && newItem.IsOverride)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match,
                    $"{newItem.Description} has added the override keyword");
            }
        }

        private static IEnumerable<ComparisonResult> EvaluateScopeChanges(ItemMatch<IPropertyDefinition> match)
        {
            var oldScope = DetermineScopeMessage(match.OldItem.Scope);
            var newScope = DetermineScopeMessage(match.NewItem.Scope);

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
    }
}