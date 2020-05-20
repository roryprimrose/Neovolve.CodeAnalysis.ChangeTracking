namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public class Change
    {
        public ChangeType ChangeType { get; set; } = ChangeType.None;

        public MemberDefinition? NewMember { get; set; } = null;

        public MemberDefinition? OldMember { get; set; } = null;

        public string Message { get; set; } = string.Empty;
    }
}