namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using ModelBuilder;

    public class CompilerModule : IConfigurationModule
    {
        public void Configure(IBuildConfiguration configuration)
        {
            configuration.AddCreationRule<MemberDefinition>(x => x.IsPublic, true, 100)
                .AddCreationRule<PropertyDefinition>(x => x.IsPublic, true, 100)
                .AddCreationRule<PropertyDefinition>(x => x.CanRead, true, 100)
                .AddCreationRule<PropertyDefinition>(x => x.CanWrite, true, 100)
                .AddIgnoreRule<AttributeDefinition>(x => x.Attributes);
        }
    }
}