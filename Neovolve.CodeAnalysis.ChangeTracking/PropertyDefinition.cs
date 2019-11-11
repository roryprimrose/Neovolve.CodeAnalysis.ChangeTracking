namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public class PropertyDefinition : MemberDefinition
    {
        public bool CanRead { get; set; }

        public bool CanWrite { get; set; }
    }
}