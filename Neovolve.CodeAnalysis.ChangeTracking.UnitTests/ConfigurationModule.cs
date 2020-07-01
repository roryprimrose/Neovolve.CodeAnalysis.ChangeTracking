namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using ModelBuilder;

    public class ConfigurationModule : IConfigurationModule
    {
        public void Configure(IBuildConfiguration configuration)
        {
            configuration.AddValueGenerator<SemVerChangeTypeValueGenerator>();
            configuration.AddTypeCreator<ComparisonResultTypeCreator>();
        }
    }
}