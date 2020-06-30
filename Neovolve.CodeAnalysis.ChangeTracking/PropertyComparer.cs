namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using EnsureThat;

    public class PropertyComparer : MemberComparer
    {
        public override ComparisonResult Compare(DefinitionMatch match)
        {
            Ensure.Any.IsNotNull(match, nameof(match));

            var oldProperty = (OldPropertyDefinition) match.OldDefinition;
            var newProperty = (OldPropertyDefinition) match.NewDefinition;

            var result = base.Compare(match);

            if (result.ChangeType == SemVerChangeType.Breaking)
            {
                // Doesn't matter if the property accessibility indicates feature or no change, breaking trumps everything
                return result;
            }

            if (oldProperty.IsVisible == false)
            {
                // The property is either still not public or now becoming public
                // It doesn't matter if the accessors have been changed to be less visible
                return result;
            }

            if (oldProperty.CanRead == newProperty.CanRead
                && oldProperty.CanWrite == newProperty.CanWrite)
            {
                // The accessibility of the property get/set members are equal so the changeType already calculated will be accurate
                return result;
            }

            // Calculate breaking changes
            if (oldProperty.CanRead
                && newProperty.CanRead == false)
            {
                var message = oldProperty + " removed get accessor availability";

                return ComparisonResult.MemberChanged(SemVerChangeType.Breaking, match,
                    message);
            }

            if (oldProperty.CanWrite
                && newProperty.CanWrite == false)
            {
                var message = oldProperty + " removed set accessor availability";

                return ComparisonResult.MemberChanged(SemVerChangeType.Breaking, match,
                    message);
            }

            // Only other possible scenario at this point is that the old property couldn't read/write but the new property can
            var accessorMessage = oldProperty + " get and/or set is now available";

            return ComparisonResult.MemberChanged(SemVerChangeType.Feature, match,
                accessorMessage);
        }

        public override bool IsSupported(OldMemberDefinition member)
        {
            Ensure.Any.IsNotNull(member, nameof(member));

            return member.GetType() == typeof(OldPropertyDefinition);
        }
    }
}