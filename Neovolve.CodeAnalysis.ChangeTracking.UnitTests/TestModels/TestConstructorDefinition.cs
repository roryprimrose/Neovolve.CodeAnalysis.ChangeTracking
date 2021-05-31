namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TestConstructorDefinition : IConstructorDefinition
    {
        public AccessModifiers AccessModifiers { get; set; } = AccessModifiers.Public;
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; set; } = new List<TestAttributeDefinition>();
        public string DeclaredModifiers { get; set; } = "public";
        public ITypeDefinition DeclaringType { get; set; } = new TestClassDefinition();
        public string FullName { get; set; } = Guid.NewGuid().ToString();
        public string FullRawName { get; set; } = Guid.NewGuid().ToString();
        public bool IsVisible { get; set; } = true;
        public DefinitionLocation Location { get; set; } = new(string.Empty, 0, 0);
        public ConstructorModifiers Modifiers { get; } = ConstructorModifiers.None;
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public IReadOnlyCollection<IParameterDefinition> Parameters { get; } = new List<IParameterDefinition>();
        public string RawName { get; set; } = Guid.NewGuid().ToString();
        public string ReturnType { get; set; } = string.Empty;
    }
}