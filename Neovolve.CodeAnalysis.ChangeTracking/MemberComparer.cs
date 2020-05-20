namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Diagnostics;
    using EnsureThat;

    public class MemberComparer : IMemberComparer
    {
        // TODO: Add logging here to explain how a change was identified
        public virtual SemVerChangeType Compare(MemberMatch match)
        {
            Ensure.Any.IsNotNull(match, nameof(match));

            if (string.Equals(match.OldMember.Namespace, match.NewMember.Namespace, StringComparison.Ordinal) == false)
            {
                throw new InvalidOperationException("The two members cannot be compared because they have different Namespace values.");
            }
            
            if (string.Equals(match.OldMember.OwningType, match.NewMember.OwningType, StringComparison.Ordinal) == false)
            {
                throw new InvalidOperationException("The two members cannot be compared because they have different OwningType values.");
            }
            
            if (string.Equals(match.OldMember.Name, match.NewMember.Name, StringComparison.Ordinal) == false)
            {
                throw new InvalidOperationException("The two members cannot be compared because they have different Name values.");
            }

            if (match.OldMember.IsPublic == false
                && match.NewMember.IsPublic == false)
            {
                // It doesn't matter if there is a change to the return type, the member isn't visible anyway
                return SemVerChangeType.None;
            }

            if (match.OldMember.IsPublic
                && match.NewMember.IsPublic == false)
            {
                // The member was public but isn't now, breaking change
                return SemVerChangeType.Breaking;
            }

            if (match.OldMember.IsPublic == false
                && match.NewMember.IsPublic)
            {
                // The member return type may have changed, but the member is only now becoming public
                // This is a feature because the public API didn't break even if the return type has changed
                return SemVerChangeType.Feature;
            }

            Debug.Assert(match.OldMember.IsPublic);
            Debug.Assert(match.NewMember.IsPublic);

            // At this point both the old member and the new member are public
            if (match.OldMember.ReturnType.Equals(match.NewMember.ReturnType, StringComparison.Ordinal) == false)
            {
                return SemVerChangeType.Breaking;
            }

            return SemVerChangeType.None;
        }

        public virtual bool IsSupported(MemberDefinition member)
        {
            Ensure.Any.IsNotNull(member, nameof(member));

            // We don't want to support derived types here
            return member.GetType() == typeof(MemberDefinition);
        }
    }
}