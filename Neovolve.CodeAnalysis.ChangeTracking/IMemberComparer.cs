namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public interface IMemberComparer
    {
        ChangeType Compare(MemberDefinition oldMember, MemberDefinition newMember);

        bool IsSupported(MemberDefinition member);
    }
}