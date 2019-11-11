namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using EnsureThat;

    public class MemberMatch
    {
        public MemberMatch(MemberDefinition oldMember, MemberDefinition newMember)
        {
            Ensure.Any.IsNotNull(oldMember, nameof(oldMember));
            Ensure.Any.IsNotNull(newMember, nameof(newMember));

            OldMember = oldMember;
            NewMember = newMember;
        }

        public MemberDefinition NewMember { get; }

        public MemberDefinition OldMember { get; }
    }
}