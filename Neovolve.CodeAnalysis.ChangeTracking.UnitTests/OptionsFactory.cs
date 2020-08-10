namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
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