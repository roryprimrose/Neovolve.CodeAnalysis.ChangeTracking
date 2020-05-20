namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using ModelBuilder;

    public class ConfigurationModule : IConfigurationModule
    {
        public void Configure(IBuildConfiguration configuration)
        {
            configuration.AddCreationRule<MemberDefinition>(x => x.IsPublic, true, 100);

            configuration.AddCreationRule<PropertyDefinition>(x => x.IsPublic, true, 100);
            configuration.AddCreationRule<PropertyDefinition>(x => x.CanRead, true, 100);
            configuration.AddCreationRule<PropertyDefinition>(x => x.CanWrite, true, 100);
            configuration.AddCreationRule<PropertyDefinition>(x => x.MemberType, MemberType.Property, 100);

            configuration.AddCreationRule<AttributeDefinition>(x => x.MemberType, MemberType.Attribute, 100);
            configuration.AddIgnoreRule<AttributeDefinition>(x => x.Attributes);
        }
    }
}