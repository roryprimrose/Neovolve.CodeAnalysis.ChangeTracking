namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public class PropertyDefinition : NodeDefinition
    {
        public bool CanRead { get; set; }

        public bool CanWrite { get; set; }
    }
}