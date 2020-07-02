namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using EnsureThat;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TypeComparer : ITypeComparer
    {
        private readonly IPropertyMatchProcessor _propertyProcessor;

        public TypeComparer(IPropertyMatchProcessor propertyProcessor)
        {
            _propertyProcessor = propertyProcessor ?? throw new ArgumentNullException(nameof(propertyProcessor));
        }

        public IEnumerable<ComparisonResult> CompareTypes(ItemMatch<ITypeDefinition> match, ComparerOptions options)
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

            // Check for a change in type
            if (match.OldItem.GetType() != match.NewItem.GetType())
            {
                var newType = DetermineTypeName(match.NewItem);

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{match.OldItem.Description} has changed to {newType}");

                // This is a fundamental change to the type. No point continuing to identify differences
                yield break;
            }

            foreach (var comparisonResult in EvaluateScopeChanges(match))
            {
                yield return comparisonResult;
            }

            foreach (var comparisonResult in EvaluateModifierChanges(match))
            {
                yield return comparisonResult;
            }

            foreach (var result in EvaluatePropertyChanges(match, options))
            {
                yield return result;
            }

            // Compare the following:
            // generic type definitions
            // implemented types
            // generic constraints
            // attributes
            // fields

            yield return ComparisonResult.NoChange(match);
        }

        private IEnumerable<ComparisonResult> EvaluatePropertyChanges(ItemMatch<ITypeDefinition> match, ComparerOptions options)
        {
            var oldProperties = match.OldItem.Properties;
            var newProperties = match.NewItem.Properties;

            return _propertyProcessor.CalculateChanges(oldProperties, newProperties, options);
        }

        private static string DetermineScopeMessage(string scope)
        {
            if (string.IsNullOrWhiteSpace(scope))
            {
                return "(implicit) private";
            }

            return scope;
        }

        private static string DetermineTypeName(ITypeDefinition item)
        {
            if (item is IClassDefinition)
            {
                return "class";
            }

            if (item is IInterfaceDefinition)
            {
                return "interface";
            }

            throw new NotSupportedException("Unknown type provided");
        }

        private static IEnumerable<ComparisonResult> EvaluateModifierChanges(ItemMatch<ITypeDefinition> match)
        {
            var oldClass = match.OldItem as IClassDefinition;

            if (oldClass == null)
            {
                yield break;
            }

            var newClass = (IClassDefinition) match.NewItem;

            if (oldClass.IsAbstract
                && newClass.IsAbstract == false)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match,
                    $"{oldClass.Description} has removed the abstract keyword");
            }
            else if (oldClass.IsAbstract == false
                     && newClass.IsAbstract)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{oldClass.Description} has added the abstract keyword");
            }

            if (oldClass.IsSealed
                && newClass.IsSealed == false)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match,
                    $"{oldClass.Description} has removed the sealed keyword");
            }
            else if (oldClass.IsSealed == false
                     && newClass.IsSealed)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{oldClass.Description} has added the sealed keyword");
            }

            if (oldClass.IsStatic
                && newClass.IsStatic == false)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{oldClass.Description} has removed the static keyword");
            }
            else if (oldClass.IsStatic == false
                     && newClass.IsStatic)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{oldClass.Description} has added the static keyword");
            }
        }

        private static IEnumerable<ComparisonResult> EvaluateScopeChanges(ItemMatch<ITypeDefinition> match)
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