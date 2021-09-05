namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class GitHubMarkdownMessageFormatter : DefaultMessageFormatter
    {
        public GitHubMarkdownMessageFormatter(IIdentifierFormatter identifierFormatter) : base(identifierFormatter)
        {
        }

        protected override string FormatIdentifier(IItemDefinition definition, ItemFormatType formatType)
        {
            return "`" + base.FormatIdentifier(definition, formatType) + "`";
        }

        protected override string FormatNewValue(IItemDefinition definition, ItemFormatType formatType, string? value)
        {
            return "`" + base.FormatNewValue(definition, formatType, value) + "`";
        }

        protected override string FormatOldValue(IItemDefinition definition, ItemFormatType formatType, string? value)
        {
            return "`" + base.FormatOldValue(definition, formatType, value) + "`";
        }
    }
}