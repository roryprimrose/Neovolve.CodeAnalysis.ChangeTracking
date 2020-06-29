namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using ModelBuilder;

    public class ConfigurationModule : IConfigurationModule
    {
        public void Configure(IBuildConfiguration configuration)
        {
            configuration.AddCreationRule<OldMemberDefinition>(x => x.IsVisible, true, 100);

            configuration.AddCreationRule<OldPropertyDefinition>(x => x.IsVisible, true, 100);
            configuration.AddCreationRule<OldPropertyDefinition>(x => x.CanRead, true, 100);
            configuration.AddCreationRule<OldPropertyDefinition>(x => x.CanWrite, true, 100);
            configuration.AddCreationRule<OldPropertyDefinition>(x => x.MemberType, MemberType.Property, 100);

            configuration.AddCreationRule<OldAttributeDefinition>(x => x.MemberType, MemberType.Attribute, 100);
            configuration.AddIgnoreRule<OldAttributeDefinition>(x => x.Attributes);

            configuration.AddValueGenerator<SemVerChangeTypeValueGenerator>();
            configuration.AddTypeCreator<ComparisonResultTypeCreator>();
        }
    }
}