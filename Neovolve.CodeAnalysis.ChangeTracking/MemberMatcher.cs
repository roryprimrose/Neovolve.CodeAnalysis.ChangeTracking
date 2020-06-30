namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using EnsureThat;

    public class MemberMatcher : IMemberMatcher
    {
        public virtual DefinitionMatch? GetMatch(OldMemberDefinition oldMember, OldMemberDefinition newMember)
        {
            Ensure.Any.IsNotNull(oldMember, nameof(oldMember));
            Ensure.Any.IsNotNull(newMember, nameof(newMember));

            if (!string.Equals(oldMember.Namespace, newMember.Namespace, StringComparison.Ordinal))
            {
                return null;
            }

            if (!string.Equals(oldMember.OwningType, newMember.OwningType, StringComparison.Ordinal))
            {
                return null;
            }

            if (!string.Equals(oldMember.Name, newMember.Name, StringComparison.Ordinal))
            {
                return null;
            }

            return new DefinitionMatch(oldMember, newMember);
        }

        public virtual bool IsSupported(OldMemberDefinition member)
        {
            Ensure.Any.IsNotNull(member, nameof(member));

            var nodeType = member.GetType();

            if (nodeType == typeof(OldPropertyDefinition))
            {
                return true;
            }

            // We don't want to support any other derived types here
            return nodeType == typeof(OldMemberDefinition);
        }
    }
}