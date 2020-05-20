namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using EnsureThat;

    public class MemberMatcher : IMemberMatcher
    {
        public virtual MemberMatch? GetMatch(MemberDefinition oldMember, MemberDefinition newMember)
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

            return new MemberMatch(oldMember, newMember);
        }

        public virtual bool IsSupported(MemberDefinition member)
        {
            Ensure.Any.IsNotNull(member, nameof(member));

            var nodeType = member.GetType();

            if (nodeType == typeof(PropertyDefinition))
            {
                return true;
            }

            // We don't want to support any other derived types here
            return nodeType == typeof(MemberDefinition);
        }
    }
}