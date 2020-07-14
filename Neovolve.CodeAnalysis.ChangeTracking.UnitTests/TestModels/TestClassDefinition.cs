namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TestClassDefinition : TestTypeDefinition, IClassDefinition
    {
        public IReadOnlyCollection<IFieldDefinition> Fields { get; set; } = new List<IFieldDefinition>();
        public bool IsAbstract { get; set; } = false;
        public bool IsPartial { get; set; } = false;
        public bool IsSealed { get; set; } = false;
        public bool IsStatic { get; set; } = false;
        public ClassModifiers Modifiers { get; set;  } = ClassModifiers.None;
    }
}