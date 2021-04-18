namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TestInterfaceDefinition : TestTypeDefinition, IInterfaceDefinition
    {
        public InterfaceModifiers Modifiers => InterfaceModifiers.None;
    }
}