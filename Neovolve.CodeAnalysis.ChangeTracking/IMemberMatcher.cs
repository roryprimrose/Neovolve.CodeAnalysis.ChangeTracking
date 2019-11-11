namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public interface IMemberMatcher
    {
        MemberMatch GetMatch(MemberDefinition oldMember, MemberDefinition newMember);

        bool IsSupported(MemberDefinition member);
    }
}