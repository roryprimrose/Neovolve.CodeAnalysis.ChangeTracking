namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TestPropertyAccessorDefinition : IPropertyAccessorDefinition
    {
        public string Description { get; set; } = Guid.NewGuid().ToString();
        public DefinitionLocation Location { get; set; } = new DefinitionLocation(string.Empty, 0, 0);
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; set; } = new List<IAttributeDefinition>();
        public string FullName { get; set; } = Guid.NewGuid().ToString();
        public string FullRawName { get; set; } = Guid.NewGuid().ToString();
        public bool IsVisible { get; set; } = true;
        public string RawName { get; set; } = Guid.NewGuid().ToString();
        public PropertyAccessorAccessModifier AccessModifier { get; set; } = PropertyAccessorAccessModifier.None;
    }
}