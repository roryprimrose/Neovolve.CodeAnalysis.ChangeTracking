namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public abstract class TestTypeDefinition : ITypeDefinition
    {
        public void MergePartialType(ITypeDefinition partialType)
        {
        }

        public AccessModifiers AccessModifiers { get; set; } = AccessModifiers.Public;
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; set; } = new List<TestAttributeDefinition>();

        public IReadOnlyCollection<IClassDefinition> ChildClasses { get; set; } = new List<TestClassDefinition>();
        public IReadOnlyCollection<IEnumDefinition> ChildEnums { get; set; } = new List<TestEnumDefinition>();

        public IReadOnlyCollection<IInterfaceDefinition> ChildInterfaces { get; set; } =
            new List<TestInterfaceDefinition>();

        public IReadOnlyCollection<IStructDefinition> ChildStructs { get; set; } = new List<TestStructDefinition>();

        public IReadOnlyCollection<IBaseTypeDefinition> ChildTypes { get; set; } = new List<IBaseTypeDefinition>();
        public string DeclaredModifiers { get; set; } = "public";
        public ITypeDefinition? DeclaringType { get; set; } = null;
        public string FullName { get; set; } = Guid.NewGuid().ToString();
        public string FullRawName { get; set; } = Guid.NewGuid().ToString();

        public IReadOnlyCollection<IConstraintListDefinition> GenericConstraints { get; set; } =
            new List<TestConstraintListDefinition>();

        public IReadOnlyCollection<string> GenericTypeParameters { get; set; } = new List<string>();
        public IReadOnlyCollection<string> ImplementedTypes { get; set; } = new List<string>();
        public bool IsVisible { get; set; } = true;
        public DefinitionLocation Location { get; set; } = new(string.Empty, 0, 0);
        public IReadOnlyCollection<IMethodDefinition> Methods { get; set; } = new List<TestMethodDefinition>();
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public string Namespace { get; set; } = Guid.NewGuid().ToString();
        public IReadOnlyCollection<IPropertyDefinition> Properties { get; set; } = new List<TestPropertyDefinition>();
        public string RawName { get; set; } = Guid.NewGuid().ToString();
    }
}