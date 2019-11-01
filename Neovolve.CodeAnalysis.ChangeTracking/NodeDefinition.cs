namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;

    public abstract class NodeDefinition
    {
        public ICollection<AttributeDefinition> Attributes { get; } = new List<AttributeDefinition>();

        public string Name { get; set; }
    }
}