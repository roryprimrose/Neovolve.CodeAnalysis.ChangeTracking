﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TestEnumDefinition : IEnumDefinition
    {
        public EnumAccessModifiers AccessModifiers { get; set; } = EnumAccessModifiers.Public;
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; set; } = new List<TestAttributeDefinition>();
        public string DeclaredModifiers { get; set; } = "public";
        public ITypeDefinition? DeclaringType { get; set; } = new TestClassDefinition();
        public string FullName { get; set; } = Guid.NewGuid().ToString();
        public string FullRawName { get; set; } = Guid.NewGuid().ToString();
        public IReadOnlyCollection<string> ImplementedTypes { get; set; } = new List<string>();
        public bool IsVisible { get; set; } = true;
        public DefinitionLocation Location { get; set; } = new(string.Empty, 0, 0);
        public IReadOnlyCollection<IEnumMemberDefinition> Members { get; set; } = new List<TestEnumMemberDefinition>();
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public string Namespace { get; set; } = Guid.NewGuid().ToString();
        public string RawName { get; set; } = Guid.NewGuid().ToString();
    }
}