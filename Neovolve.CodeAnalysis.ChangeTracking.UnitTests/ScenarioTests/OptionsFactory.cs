namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ScenarioTests
{
    internal static class OptionsFactory
    {
        public static ComparerOptions BuildOptions()
        {
            var messageFormatter = new GitHubMarkdownMessageFormatter();

            var options = ComparerOptions.Default;

            options.MessageFormatter = messageFormatter;

            return options;
        }
    }
}