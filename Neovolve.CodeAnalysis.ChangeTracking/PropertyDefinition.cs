namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public class PropertyDefinition : NodeDefinition
    {
        public bool CanRead { get; set; }

        public bool CanWrite { get; set; }

        public string ClassName { get; set; }

        public string DataType { get; set; }

        public bool IsPublic { get; set; }

        public string Namespace { get; set; }
    }
}