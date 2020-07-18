namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;

    public class ConfigurationModule : IConfigurationModule
    {
        public void Configure(IBuildConfiguration configuration)
        {
            configuration.Mapping<IArgumentDefinition, TestArgumentDefinition>();
            configuration.Mapping<IAttributeDefinition, TestAttributeDefinition>();
            configuration.Mapping<IClassDefinition, TestClassDefinition>();
            configuration.Mapping<IConstraintListDefinition, TestConstraintListDefinition>();
            configuration.Mapping<IFieldDefinition, TestFieldDefinition>();
            configuration.Mapping<IInterfaceDefinition, TestInterfaceDefinition>();
            configuration.Mapping<IPropertyDefinition, TestPropertyDefinition>();
            configuration.Mapping<ITypeDefinition, TestClassDefinition>();
            configuration.Mapping<IMemberDefinition, TestPropertyDefinition>();
            configuration.Mapping<IElementDefinition, TestClassDefinition>();
            configuration.Mapping<IItemDefinition, TestClassDefinition>();
            configuration.AddValueGenerator<SemVerChangeTypeValueGenerator>();
        }
    }
}