namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    internal static class OptionsFactory
    {
        public static ComparerOptions BuildOptions()
        {
            var identifierFormatter = new GitHubMarkdownIdentifierFormatter();
            var messageFormatter = new GitHubMarkdownMessageFormatter(identifierFormatter);

            var options = ComparerOptions.Default;

            options.MessageFormatter = messageFormatter;

            return options;
        }
    }
}