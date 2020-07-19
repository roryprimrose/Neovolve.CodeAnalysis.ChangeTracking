namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IMessageFormatter
    {
        string FormatItemAddedMessage(IItemDefinition definition, FormatArguments arguments);
        string FormatItemChangedMessage<T>(ItemMatch<T> match, FormatArguments arguments) where T : IItemDefinition;
        string FormatItemRemovedMessage(IItemDefinition definition, FormatArguments arguments);
    }
}