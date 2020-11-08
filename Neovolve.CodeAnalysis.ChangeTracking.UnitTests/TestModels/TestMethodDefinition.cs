namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TestMethodDefinition : IMethodDefinition
    {
        public DefinitionLocation Location { get; set; } = new DefinitionLocation(string.Empty, 0, 0);
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; set; } = new List<TestAttributeDefinition>();
        public string DeclaredModifiers { get; set; } = Guid.NewGuid().ToString();
        public string FullName { get; set; } = Guid.NewGuid().ToString();
        public string FullRawName { get; set; } = Guid.NewGuid().ToString();
        public bool IsVisible { get; set; } = true;
        public string RawName { get; set; } = Guid.NewGuid().ToString();
        public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;
        public ITypeDefinition DeclaringType { get; set; } = new TestClassDefinition();
        public string ReturnType { get; set; } = Guid.NewGuid().ToString();
        public IReadOnlyCollection<IConstraintListDefinition> GenericConstraints { get; set; } = new List<TestConstraintListDefinition>();
        public IReadOnlyCollection<string> GenericTypeParameters { get; set; } = new List<string>();
        public MethodModifiers Modifiers { get; set; } = MethodModifiers.None;
        public IReadOnlyCollection<IParameterDefinition> Parameters { get; set; } = new List<TestParameterDefinition>();
    }
}