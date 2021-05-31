namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TestStructDefinition : TestTypeDefinition, IStructDefinition
    {
        public IReadOnlyCollection<IConstructorDefinition> Constructors { get; } =
            new List<TestConstructorDefinition>();

        public IReadOnlyCollection<IFieldDefinition> Fields { get; set; } = new List<TestFieldDefinition>();
        public StructModifiers Modifiers { get; set; } = StructModifiers.None;
    }
}