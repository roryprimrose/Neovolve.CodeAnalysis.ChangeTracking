namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public interface IMemberComparer
    {
        ChangeType Compare(MemberMatch match);

        bool IsSupported(MemberDefinition member);
    }
}