namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TestPropertyDefinition : IPropertyDefinition
    {
        public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; set; } = new List<IAttributeDefinition>();
        public bool CanRead { get; set; } = true;
        public bool CanWrite { get; set; } = true;
        public ITypeDefinition DeclaringType { get; set; } = new TestClassDefinition();
        public string Description { get; set; } = Guid.NewGuid().ToString();
        public string FullName { get; set; } = Guid.NewGuid().ToString();
        public string FullRawName { get; set; } = Guid.NewGuid().ToString();
        public bool IsAbstract { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public bool IsOverride { get; set; } = false;
        public bool IsSealed { get; set; } = false;
        public bool IsStatic { get; set; } = false;
        public bool IsVirtual { get; set; } = false;
        public bool IsVisible { get; set; } = true;
        public DefinitionLocation Location { get; set; } = new DefinitionLocation(string.Empty, 0, 0);
        public string Modifiers { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public string RawName { get; set; } = Guid.NewGuid().ToString();
        public string ReturnType { get; set; } = Guid.NewGuid().ToString();
    }
}