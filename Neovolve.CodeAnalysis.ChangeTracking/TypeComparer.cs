namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using EnsureThat;

    public class TypeComparer : ITypeComparer
    {
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

            if (match.OldItem.IsVisible
                && match.NewItem.IsVisible == false)
            {
                // The member was visible but isn't now, breaking change
                var message = match.OldItem + " changed scope from public";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    message);
            }

            if (match.OldItem.IsVisible == false
                && match.NewItem.IsVisible)
            {
                // The member return type may have changed, but the member is only now becoming public
                // This is a feature because the public API didn't break even if the return type has changed
                var message = match.OldItem + " changed scope to public";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match,
                    message);
            }

            // Compare the following:
            // static keyword
            // generic type definitions
            // implemented types
            // generic constraints
            // attributes
            // fields
            // properties

            yield return ComparisonResult.NoChange(match);
        }
    }
}