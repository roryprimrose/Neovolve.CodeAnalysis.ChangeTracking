namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IIdentifierFormatter
    {
        string FormatIdentifier(IItemDefinition definition, ItemFormatType formatType);
    }
}