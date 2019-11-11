namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using EnsureThat;

    public class PropertyComparer : MemberComparer
    {
        public override ChangeType Compare(MemberDefinition oldMember, MemberDefinition newMember)
        {
            Ensure.Any.IsNotNull(oldMember, nameof(oldMember));
            Ensure.Type.IsOfType(oldMember, typeof(PropertyDefinition), nameof(oldMember));
            Ensure.Any.IsNotNull(newMember, nameof(newMember));
            Ensure.Type.IsOfType(newMember, typeof(PropertyDefinition), nameof(newMember));

            var oldProperty = (PropertyDefinition) oldMember;
            var newProperty = (PropertyDefinition) newMember;

            var changeType = base.Compare(oldProperty, newProperty);

            if (changeType == ChangeType.Breaking)
            {
                // Doesn't matter if the property accessibility indicates feature or no change, breaking trumps everything
                return changeType;
            }

            if (oldProperty.IsPublic == false)
            {
                // The property is either still not public or now becoming public
                // It doesn't matter if the accessors have been changed to be less visible
                return changeType;
            }

            if (oldProperty.CanRead == newProperty.CanRead
                && oldProperty.CanWrite == newProperty.CanWrite)
            {
                // The accessibility of the property get/set members are equal so the changeType already calculated will be accurate
                return changeType;
            }

            // Calculate breaking changes
            if (oldProperty.CanRead
                && newProperty.CanRead == false)
            {
                return ChangeType.Breaking;
            }

            if (oldProperty.CanWrite
                && newProperty.CanWrite == false)
            {
                return ChangeType.Breaking;
            }

            // Only other possible scenario at this point is that the old property couldn't read/write but the new property can
            return ChangeType.Feature;
        }

        public override bool IsSupported(MemberDefinition member)
        {
            Ensure.Any.IsNotNull(member, nameof(member));

            return member.GetType() == typeof(PropertyDefinition);
        }
    }
}