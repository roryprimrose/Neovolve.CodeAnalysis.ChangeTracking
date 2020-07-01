namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using EnsureThat;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyComparer : IPropertyComparer
    {
        public IEnumerable<ComparisonResult> CompareTypes(ItemMatch<PropertyDefinition> match, ComparerOptions options)
        {
            Ensure.Any.IsNotNull(match, nameof(match));

            var oldProperty = match.OldItem;
            var newProperty = match.NewItem;

            //var result = base.Compare(match);

            //if (result.ChangeType == SemVerChangeType.Breaking)
            //{
            //    // Doesn't matter if the property accessibility indicates feature or no change, breaking trumps everything
            //    return result;
            //}

            //if (oldProperty.IsVisible == false)
            //{
            //    // The property is either still not public or now becoming public
            //    // It doesn't matter if the accessors have been changed to be less visible
            //    return result;
            //}

            //if (oldProperty.CanRead == newProperty.CanRead
            //    && oldProperty.CanWrite == newProperty.CanWrite)
            //{
            //    // The accessibility of the property get/set members are equal so the changeType already calculated will be accurate
            //    return result;
            //}

            // Calculate breaking changes
            if (oldProperty.CanRead
                && newProperty.CanRead == false)
            {
                var message = oldProperty + " removed get accessor availability";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);
            }
            else if (oldProperty.CanWrite
                && newProperty.CanWrite == false)
            {
                var message = oldProperty + " removed set accessor availability";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);
            }

            // Only other possible scenario at this point is that the old property couldn't read/write but the new property can
            var accessorMessage = oldProperty + " get and/or set is now available";

            yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match, accessorMessage);
        }
    }
}