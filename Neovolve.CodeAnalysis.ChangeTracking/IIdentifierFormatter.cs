namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IIdentifierFormatter" />
    ///     interface defines the members to format the identifier of an item.
    /// </summary>
    public interface IIdentifierFormatter
    {
        /// <summary>
        ///     Formats the identifier of the specified item.
        /// </summary>
        /// <param name="definition">The item to format.</param>
        /// <param name="formatType">The item format type.</param>
        /// <returns>The formatted identifier for the item.</returns>
        string FormatIdentifier(IItemDefinition definition, ItemFormatType formatType);
    }
}