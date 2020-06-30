namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public interface IMemberComparer
    {
        ComparisonResult Compare(DefinitionMatch match);

        bool IsSupported(OldMemberDefinition member);
    }
}