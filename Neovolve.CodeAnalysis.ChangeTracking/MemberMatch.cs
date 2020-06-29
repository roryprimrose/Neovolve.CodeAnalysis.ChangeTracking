namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using EnsureThat;

    public class MemberMatch
    {
        public MemberMatch(OldMemberDefinition oldMember, OldMemberDefinition newMember)
        {
            Ensure.Any.IsNotNull(oldMember, nameof(oldMember));
            Ensure.Any.IsNotNull(newMember, nameof(newMember));

            OldMember = oldMember;
            NewMember = newMember;
        }

        public OldMemberDefinition NewMember { get; }

        public OldMemberDefinition OldMember { get; }
    }
}