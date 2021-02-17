namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    internal class TestArgumentDefinition : IArgumentDefinition
    {
        public ArgumentType ArgumentType { get; set; } = ArgumentType.Ordinal;
        public string Declaration { get; set; } = Guid.NewGuid().ToString();
        public DefinitionLocation Location { get; set; } = new(string.Empty, 0, 0);
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public int? OrdinalIndex { get; set; } = null;
        public string ParameterName { get; set; } = Guid.NewGuid().ToString();
        public string Value { get; set; } = Guid.NewGuid().ToString();
    }
}