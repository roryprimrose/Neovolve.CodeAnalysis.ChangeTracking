namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IMessageFormatter
    {
        string FormatMessage(IItemDefinition definition, FormatArguments arguments);
    }
}