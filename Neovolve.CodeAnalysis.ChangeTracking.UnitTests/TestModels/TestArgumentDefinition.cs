﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    internal class TestArgumentDefinition : IArgumentDefinition
    {
        public ArgumentType ArgumentType { get; set; } = ArgumentType.Ordinal;
        public string Description { get; set; } = Guid.NewGuid().ToString();
        public DefinitionLocation Location { get; set; } = new DefinitionLocation(string.Empty, 0, 0);
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public int? OrdinalIndex { get; } = null;
        public string Value { get; set; } = Guid.NewGuid().ToString();
    }
}