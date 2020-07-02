namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public abstract class TestTypeDefinition : ITypeDefinition
    {
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; set; } = new List<IAttributeDefinition>();
        public IReadOnlyCollection<IClassDefinition> ChildClasses { get; set; } = new List<IClassDefinition>();

        public IReadOnlyCollection<IInterfaceDefinition> ChildInterfaces { get; set; } =
            new List<IInterfaceDefinition>();

        public IReadOnlyCollection<ITypeDefinition> ChildTypes { get; set; } = new List<ITypeDefinition>();
        public ITypeDefinition? DeclaringType { get; set; } = null;
        public string Description { get; set; } = Guid.NewGuid().ToString();
        public string FullName { get; set; } = Guid.NewGuid().ToString();

        public IReadOnlyCollection<IConstraintListDefinition> GenericConstraints { get; set; } =
            new List<IConstraintListDefinition>();

        public IReadOnlyCollection<string> GenericTypeParameters { get; } = new List<string>();

        public IReadOnlyCollection<string> ImplementedTypes { get; set; } = new List<string>();
        public bool IsVisible { get; set; } = true;
        public DefinitionLocation Location { get; set; } = new DefinitionLocation(string.Empty, 0, 0);
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public string Namespace { get; set; } = Guid.NewGuid().ToString();
        public IReadOnlyCollection<IPropertyDefinition> Properties { get; set; } = new List<IPropertyDefinition>();
        public string Scope { get; set; } = Guid.NewGuid().ToString();
    }
}