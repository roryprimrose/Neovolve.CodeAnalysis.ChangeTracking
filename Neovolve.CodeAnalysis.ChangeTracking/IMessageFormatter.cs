namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IMessageFormatter
    {
        string FormatItem(IItemDefinition definition, ItemFormatType formatType, FormatArguments arguments);

        string FormatMatch<T>(ItemMatch<T> match, ItemFormatType formatType, FormatArguments arguments)
            where T : IItemDefinition;
    }
}