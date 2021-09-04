namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TestAttributeDefinition : IAttributeDefinition
    {
        public IReadOnlyCollection<IArgumentDefinition> Arguments { get; set; } = new List<TestArgumentDefinition>();
        public IElementDefinition DeclaringElement { get; set; } = new TestMethodDefinition();
        public DefinitionLocation Location { get; set; } = new(string.Empty, 0, 0);
        public string Name { get; set; } = Guid.NewGuid().ToString();
    }
}