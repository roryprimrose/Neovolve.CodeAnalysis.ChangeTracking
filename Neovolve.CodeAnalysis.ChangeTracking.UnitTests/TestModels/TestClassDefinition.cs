namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TestClassDefinition : TestTypeDefinition, IClassDefinition
    {
        public IReadOnlyCollection<IFieldDefinition> Fields { get; set; } = new List<IFieldDefinition>();
    }
}