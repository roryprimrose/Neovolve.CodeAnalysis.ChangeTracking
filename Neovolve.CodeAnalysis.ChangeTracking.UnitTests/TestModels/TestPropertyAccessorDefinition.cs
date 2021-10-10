namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TestPropertyAccessorDefinition : IPropertyAccessorDefinition
    {
        public PropertyAccessorAccessModifiers AccessModifiers { get; set; } = PropertyAccessorAccessModifiers.None;
        public PropertyAccessorPurpose AccessorPurpose { get; set; } = PropertyAccessorPurpose.Read;
        public PropertyAccessorType AccessorType { get; set; } = PropertyAccessorType.Get;
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; set; } = new List<TestAttributeDefinition>();
        public string DeclaredModifiers { get; set; } = string.Empty;
        public string FullName { get; set; } = Guid.NewGuid().ToString();
        public string FullRawName { get; set; } = Guid.NewGuid().ToString();
        public bool IsVisible { get; set; } = true;
        public DefinitionLocation Location { get; set; } = new(string.Empty, 0, 0);
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public string RawName { get; set; } = Guid.NewGuid().ToString();
    }
}