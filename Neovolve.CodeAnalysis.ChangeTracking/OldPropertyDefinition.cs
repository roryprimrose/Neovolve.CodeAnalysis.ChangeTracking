namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public class OldPropertyDefinition : OldMemberDefinition
    {
        public bool CanRead { get; set; }

        public bool CanWrite { get; set; }
    }
}