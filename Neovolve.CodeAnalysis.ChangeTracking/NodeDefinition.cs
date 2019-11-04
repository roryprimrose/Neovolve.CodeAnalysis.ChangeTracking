namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;

    public abstract class NodeDefinition
    {
        public ICollection<AttributeDefinition> Attributes { get; } = new List<AttributeDefinition>();

        public bool IsPublic { get; set; }

        public string Name { get; set; }

        public string Namespace { get; set; }

        public string OwningType { get; set; }

        public string ReturnType { get; set; }
    }
}