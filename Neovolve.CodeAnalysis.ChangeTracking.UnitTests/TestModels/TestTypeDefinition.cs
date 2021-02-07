namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public abstract class TestTypeDefinition : ITypeDefinition
    {
        public AccessModifiers AccessModifiers { get; set; } = AccessModifiers.Public;
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; set; } = new List<TestAttributeDefinition>();
        public IReadOnlyCollection<IClassDefinition> ChildClasses { get; set; } = new List<TestClassDefinition>();

        public IReadOnlyCollection<IInterfaceDefinition> ChildInterfaces { get; set; } =
            new List<TestInterfaceDefinition>();

        public IReadOnlyCollection<IStructDefinition> ChildStructs { get; } = new List<TestStructDefinition>();

        public IReadOnlyCollection<ITypeDefinition> ChildTypes { get; set; } = new List<TestClassDefinition>();
        public string DeclaredModifiers { get; } = "public";
        public ITypeDefinition? DeclaringType { get; set; } = null;
        public string FullName { get; set; } = Guid.NewGuid().ToString();
        public string FullRawName { get; set; } = Guid.NewGuid().ToString();

        public IReadOnlyCollection<IConstraintListDefinition> GenericConstraints { get; set; } =
            new List<TestConstraintListDefinition>();

        public IReadOnlyCollection<string> GenericTypeParameters { get; set; } = new List<string>();
        public IReadOnlyCollection<string> ImplementedTypes { get; set; } = new List<string>();
        public bool IsVisible { get; set; } = true;
        public DefinitionLocation Location { get; set; } = new DefinitionLocation(string.Empty, 0, 0);
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public string Namespace { get; set; } = Guid.NewGuid().ToString();
        public IReadOnlyCollection<IPropertyDefinition> Properties { get; set; } = new List<TestPropertyDefinition>();
        public string RawName { get; set; } = Guid.NewGuid().ToString();
    }
}