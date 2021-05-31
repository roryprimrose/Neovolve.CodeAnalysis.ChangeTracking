namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TestClassDefinition : TestTypeDefinition, IClassDefinition
    {
        public IReadOnlyCollection<IConstructorDefinition> Constructors { get; } =
            new List<TestConstructorDefinition>();

        public IReadOnlyCollection<IFieldDefinition> Fields { get; set; } = new List<TestFieldDefinition>();
        public ClassModifiers Modifiers { get; set; } = ClassModifiers.None;
    }
}