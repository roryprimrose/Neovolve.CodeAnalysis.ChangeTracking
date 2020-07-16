namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class GitHubMarkdownMessageFormatter : DefaultMessageFormatter
    {
        protected override string FormatIdentifier(IItemDefinition definition, string identifier)
        {
            return "`" + base.FormatIdentifier(definition, identifier) + "`";
        }

        protected override string FormatNewValue(IItemDefinition definition, string? value)
        {
            return "`" + base.FormatNewValue(definition, value) + "`";
        }

        protected override string FormatOldValue(IItemDefinition definition, string? value)
        {
            return "`" + base.FormatOldValue(definition, value) + "`";
        }
    }
}