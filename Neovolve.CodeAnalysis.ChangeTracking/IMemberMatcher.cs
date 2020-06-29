namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public interface IMemberMatcher
    {
        MemberMatch? GetMatch(OldMemberDefinition oldMember, OldMemberDefinition newMember);

        bool IsSupported(OldMemberDefinition member);
    }
}