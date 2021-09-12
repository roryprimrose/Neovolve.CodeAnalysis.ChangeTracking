namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class GitHubMarkdownIdentifierFormatter : DefaultIdentifierFormatter
    {
        protected override string FormatItem(IItemDefinition definition, ItemFormatType formatType)
        {
            return "`" + base.FormatItem(definition, formatType) + "`";
        }
    }
}