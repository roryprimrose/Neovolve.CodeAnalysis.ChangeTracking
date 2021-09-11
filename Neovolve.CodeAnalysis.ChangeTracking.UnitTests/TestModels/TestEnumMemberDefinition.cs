namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TestEnumMemberDefinition : IEnumMemberDefinition
    {
        public AccessModifiers AccessModifiers { get; set; } = AccessModifiers.Public;
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; set; } = new List<TestAttributeDefinition>();
        public string DeclaredModifiers { get; set; } = "public";
        public IEnumDefinition DeclaringType { get; set; } = new TestEnumDefinition();
        public string FullName { get; set; } = Guid.NewGuid().ToString();
        public string FullRawName { get; set; } = Guid.NewGuid().ToString();
        public int Index { get; set; } = new Random().Next();
        public bool IsVisible { get; set; } = true;
        public DefinitionLocation Location { get; set; } = new(string.Empty, 0, 0);
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public string RawName { get; set; } = Guid.NewGuid().ToString();
        public string Value { get; set; } = Guid.NewGuid().ToString();
    }
}