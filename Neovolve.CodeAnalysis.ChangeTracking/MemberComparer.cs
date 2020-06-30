namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using EnsureThat;

    public class MemberComparer : IMemberComparer
    {
        public virtual ComparisonResult Compare(DefinitionMatch match)
        {
            Ensure.Any.IsNotNull(match, nameof(match));

            if (string.Equals(match.OldDefinition.Namespace, match.NewDefinition.Namespace, StringComparison.Ordinal) == false)
            {
                throw new InvalidOperationException(
                    "The two members cannot be compared because they have different Namespace values.");
            }

            if (string.Equals(match.OldDefinition.OwningType, match.NewDefinition.OwningType, StringComparison.Ordinal) ==
                false)
            {
                throw new InvalidOperationException(
                    "The two members cannot be compared because they have different OwningType values.");
            }

            if (string.Equals(match.OldDefinition.Name, match.NewDefinition.Name, StringComparison.Ordinal) == false)
            {
                throw new InvalidOperationException(
                    "The two members cannot be compared because they have different Name values.");
            }

            if (match.OldDefinition.IsVisible == false
                && match.NewDefinition.IsVisible == false)
            {
                // It doesn't matter if there is a change to the return type, the member isn't visible anyway
                return ComparisonResult.NoChange(match);
            }

            if (match.OldDefinition.IsVisible
                && match.NewDefinition.IsVisible == false)
            {
                // The member was public but isn't now, breaking change
                var message = match.OldDefinition + " changed scope from public";

                return ComparisonResult.MemberChanged(SemVerChangeType.Breaking, match,
                    message);
            }

            if (match.OldDefinition.IsVisible == false
                && match.NewDefinition.IsVisible)
            {
                // The member return type may have changed, but the member is only now becoming public
                // This is a feature because the public API didn't break even if the return type has changed
                var message = match.OldDefinition + " changed scope to public";

                return ComparisonResult.MemberChanged(SemVerChangeType.Feature, match,
                    message);
            }

            // At this point both the old member and the new member are public
            if (match.OldDefinition.ReturnType.Equals(match.NewDefinition.ReturnType, StringComparison.Ordinal) == false)
            {
                var message = match.OldDefinition +
                              $" changed return type from {match.OldDefinition.ReturnType} to {match.NewDefinition.ReturnType}";

                return ComparisonResult.MemberChanged(SemVerChangeType.Breaking, match,
                    message);
            }

            return ComparisonResult.NoChange(match);
        }

        public virtual bool IsSupported(OldMemberDefinition member)
        {
            Ensure.Any.IsNotNull(member, nameof(member));

            // We don't want to support derived types here
            return member.GetType() == typeof(OldMemberDefinition);
        }
    }
}