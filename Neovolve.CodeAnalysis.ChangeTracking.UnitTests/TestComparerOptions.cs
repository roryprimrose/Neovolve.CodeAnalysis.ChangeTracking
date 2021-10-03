namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    public static class TestComparerOptions
    {
        public static ComparerOptions Default => BuildDefaultOptions();

        private static ComparerOptions BuildDefaultOptions()
        {
            var defaultOptions = ComparerOptions.Default;

            var identifierFormatter = new GitHubMarkdownIdentifierFormatter();
            var messageFormatter = new GitHubMarkdownMessageFormatter(identifierFormatter);

            defaultOptions.MessageFormatter = messageFormatter;

            return defaultOptions;
        }
    }
}