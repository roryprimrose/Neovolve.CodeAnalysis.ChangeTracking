namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public interface IMemberComparer
    {
        SemVerChangeType Compare(MemberMatch match);

        bool IsSupported(MemberDefinition member);
    }
}