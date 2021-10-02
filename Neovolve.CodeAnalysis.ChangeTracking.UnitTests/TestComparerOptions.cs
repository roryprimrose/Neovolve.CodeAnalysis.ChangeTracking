namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    public static class TestComparerOptions
    {
        public static ComparerOptions Default = BuildDefaultOptions();

        private static ComparerOptions BuildDefaultOptions()
        {
            var identifierFormatter = new GitHubMarkdownIdentifierFormatter();
            var messageFormatter = new GitHubMarkdownMessageFormatter(identifierFormatter);

            var options = new ComparerOptions
            {
                MessageFormatter = messageFormatter
            };

            return options;
        }
    }
}