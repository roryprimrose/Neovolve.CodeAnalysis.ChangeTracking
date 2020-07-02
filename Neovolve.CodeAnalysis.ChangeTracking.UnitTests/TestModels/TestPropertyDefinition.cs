﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TestPropertyDefinition : IPropertyDefinition
    {
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; set; } = new List<IAttributeDefinition>();
        public ITypeDefinition? DeclaringType { get; set; } = null;
        public string FullName { get; set; } = Guid.NewGuid().ToString();
        public bool IsVisible { get; set; } = true;
        public DefinitionLocation Location { get; set; } = new DefinitionLocation(string.Empty, 0, 0);
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public string ReturnType { get; set; } = Guid.NewGuid().ToString();
        public bool CanRead { get; set; } = true;
        public bool CanWrite { get; set; } = true;
        public string Description { get; set; } = Guid.NewGuid().ToString();
        public string Scope { get; set; } = Guid.NewGuid().ToString();
    }
}