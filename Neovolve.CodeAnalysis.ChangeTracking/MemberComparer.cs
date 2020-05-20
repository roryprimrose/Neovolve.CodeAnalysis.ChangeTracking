namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using EnsureThat;

    public class MemberComparer : IMemberComparer
    {
        public virtual ComparisonResult Compare(MemberMatch match)
        {
            Ensure.Any.IsNotNull(match, nameof(match));

            if (string.Equals(match.OldMember.Namespace, match.NewMember.Namespace, StringComparison.Ordinal) == false)
            {
                throw new InvalidOperationException(
                    "The two members cannot be compared because they have different Namespace values.");
            }

            if (string.Equals(match.OldMember.OwningType, match.NewMember.OwningType, StringComparison.Ordinal) ==
                false)
            {
                throw new InvalidOperationException(
                    "The two members cannot be compared because they have different OwningType values.");
            }

            if (string.Equals(match.OldMember.Name, match.NewMember.Name, StringComparison.Ordinal) == false)
            {
                throw new InvalidOperationException(
                    "The two members cannot be compared because they have different Name values.");
            }

            if (match.OldMember.IsPublic == false &&
                match.NewMember.IsPublic == false)
            {
                // It doesn't matter if there is a change to the return type, the member isn't visible anyway
                return ComparisonResult.NoChange(match.OldMember, match.NewMember);
            }

            if (match.OldMember.IsPublic &&
                match.NewMember.IsPublic == false)
            {
                // The member was public but isn't now, breaking change
                var message = match.OldMember + " changed scope from public";

                return ComparisonResult.MemberChanged(SemVerChangeType.Breaking, match.OldMember, match.NewMember,
                    message);
            }

            if (match.OldMember.IsPublic == false &&
                match.NewMember.IsPublic)
            {
                // The member return type may have changed, but the member is only now becoming public
                // This is a feature because the public API didn't break even if the return type has changed
                var message = match.OldMember + " changed scope to public";

                return ComparisonResult.MemberChanged(SemVerChangeType.Feature, match.OldMember, match.NewMember,
                    message);
            }

            // At this point both the old member and the new member are public
            if (match.OldMember.ReturnType.Equals(match.NewMember.ReturnType, StringComparison.Ordinal) == false)
            {
                var message = match.OldMember +
                              $" changed return type from {match.OldMember.ReturnType} to {match.NewMember.ReturnType}";

                return ComparisonResult.MemberChanged(SemVerChangeType.Breaking, match.OldMember, match.NewMember,
                    message);
            }

            return ComparisonResult.NoChange(match.OldMember, match.NewMember);
        }

        public virtual bool IsSupported(MemberDefinition member)
        {
            Ensure.Any.IsNotNull(member, nameof(member));

            // We don't want to support derived types here
            return member.GetType() == typeof(MemberDefinition);
        }
    }
}