namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Diagnostics;
    using EnsureThat;

    public class MemberComparer : IMemberComparer
    {
        // TODO: Add logging here to explain how a change was identified
        public virtual ChangeType Compare(MemberDefinition oldMember, MemberDefinition newMember)
        {
            Ensure.Any.IsNotNull(oldMember, nameof(oldMember));
            Ensure.Any.IsNotNull(newMember, nameof(newMember));

            if (string.Equals(oldMember.Namespace, newMember.Namespace, StringComparison.Ordinal) == false)
            {
                throw new InvalidOperationException("The two members cannot be compared because they have different Namespace values.");
            }
            
            if (string.Equals(oldMember.OwningType, newMember.OwningType, StringComparison.Ordinal) == false)
            {
                throw new InvalidOperationException("The two members cannot be compared because they have different OwningType values.");
            }
            
            if (string.Equals(oldMember.Name, newMember.Name, StringComparison.Ordinal) == false)
            {
                throw new InvalidOperationException("The two members cannot be compared because they have different Name values.");
            }

            if (oldMember.IsPublic == false
                && newMember.IsPublic == false)
            {
                // It doesn't matter if there is a change to the return type, the member isn't visible anyway
                return ChangeType.None;
            }

            if (oldMember.IsPublic
                && newMember.IsPublic == false)
            {
                // The member was public but isn't now, breaking change
                return ChangeType.Breaking;
            }

            if (oldMember.IsPublic == false
                && newMember.IsPublic)
            {
                // The member return type may have changed, but the member is only now becoming public
                // This is a feature because the public API didn't break even if the return type has changed
                return ChangeType.Feature;
            }

            Debug.Assert(oldMember.IsPublic);
            Debug.Assert(newMember.IsPublic);

            // At this point both the old member and the new member are public
            if (oldMember.ReturnType.Equals(newMember.ReturnType, StringComparison.Ordinal) == false)
            {
                return ChangeType.Breaking;
            }

            return ChangeType.None;
        }

        public virtual bool IsSupported(MemberDefinition member)
        {
            Ensure.Any.IsNotNull(member, nameof(member));

            // We don't want to support derived types here
            return member.GetType() == typeof(MemberDefinition);
        }
    }
}