﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TestPropertyDefinition : IPropertyDefinition
    {
        public AccessModifiers AccessModifiers { get; set; } = AccessModifiers.Public;
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; set; } = new List<TestAttributeDefinition>();
        public string DeclaredModifiers { get; } = "public";
        public ITypeDefinition DeclaringType { get; set; } = new TestClassDefinition();
        public string FullName { get; set; } = Guid.NewGuid().ToString();
        public string FullRawName { get; set; } = Guid.NewGuid().ToString();
        public IPropertyAccessorDefinition? GetAccessor { get; } = new TestPropertyAccessorDefinition();
        public bool IsVisible { get; set; } = true;
        public DefinitionLocation Location { get; set; } = new DefinitionLocation(string.Empty, 0, 0);
        public MemberModifiers Modifiers { get; set; } = MemberModifiers.None;
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public string RawName { get; set; } = Guid.NewGuid().ToString();
        public string ReturnType { get; set; } = Guid.NewGuid().ToString();
        public IPropertyAccessorDefinition? SetAccessor { get; } = new TestPropertyAccessorDefinition();
    }
}