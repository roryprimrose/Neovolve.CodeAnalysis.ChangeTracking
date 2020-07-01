namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TestAttributeDefinition : IAttributeDefinition
    {
        public IReadOnlyCollection<IArgumentDefinition> Arguments { get; set; } = new List<IArgumentDefinition>();
        public IItemDefinition DeclaredOn { get; set; } = new TestClassDefinition();
        public DefinitionLocation Location { get; set; } = new DefinitionLocation(string.Empty, 0, 0);
        public string Name { get; set; } = Guid.NewGuid().ToString();
    }
}