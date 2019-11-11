namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using ModelBuilder;

    public class CompilerModule : ICompilerModule
    {
        public void Configure(IBuildStrategyCompiler compiler)
        {
            compiler.AddCreationRule<NodeDefinition>(x => x.IsPublic, 100, true)
                .AddCreationRule<PropertyDefinition>(x => x.IsPublic, 100, true)
                .AddCreationRule<PropertyDefinition>(x => x.CanRead, 100, true)
                .AddCreationRule<PropertyDefinition>(x => x.CanWrite, 100, true)
                .AddIgnoreRule<AttributeDefinition>(x => x.Attributes);
        }
    }
}