namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IMessageFormatter
    {
        string FormatItem(IItemDefinition definition, ItemFormatType formatType, IFormatArguments arguments);

        string FormatMatch<T>(ItemMatch<T> match, ItemFormatType formatType, IFormatArguments arguments)
            where T : IItemDefinition;
    }
}