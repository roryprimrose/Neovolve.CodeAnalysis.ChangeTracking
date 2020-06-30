namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public interface IMemberMatcher
    {
        DefinitionMatch? GetMatch(OldMemberDefinition oldMember, OldMemberDefinition newMember);

        bool IsSupported(OldMemberDefinition member);
    }
}