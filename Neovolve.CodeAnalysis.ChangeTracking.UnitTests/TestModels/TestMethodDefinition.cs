﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TestMethodDefinition : IMethodDefinition
    {
        public AccessModifiers AccessModifiers { get; set; } = AccessModifiers.Public;
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; set; } = new List<TestAttributeDefinition>();
        public string DeclaredModifiers { get; set; } = Guid.NewGuid().ToString();
        public ITypeDefinition DeclaringType { get; set; } = new TestClassDefinition();
        public string FullName { get; set; } = Guid.NewGuid().ToString();
        public string FullRawName { get; set; } = Guid.NewGuid().ToString();

        public IReadOnlyCollection<IConstraintListDefinition> GenericConstraints { get; set; } =
            new List<TestConstraintListDefinition>();

        public IReadOnlyCollection<string> GenericTypeParameters { get; set; } = new List<string>();
        public bool IsVisible { get; set; } = true;
        public DefinitionLocation Location { get; set; } = new(string.Empty, 0, 0);
        public MethodModifiers Modifiers { get; set; } = MethodModifiers.None;
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public IReadOnlyCollection<IParameterDefinition> Parameters { get; set; } = new List<TestParameterDefinition>();
        public bool HasBody { get; set; } = true;
        public string RawName { get; set; } = Guid.NewGuid().ToString();
        public string ReturnType { get; set; } = Guid.NewGuid().ToString();
    }
}