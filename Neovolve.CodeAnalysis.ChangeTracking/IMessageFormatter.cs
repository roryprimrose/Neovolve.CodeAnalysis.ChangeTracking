namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IMessageFormatter" />
    ///     interface defines the members used to format a message for how an item has changed.
    /// </summary>
    public interface IMessageFormatter
    {
        /// <summary>
        ///     Formats the specified item using the provided arguments.
        /// </summary>
        /// <param name="definition">The item to format.</param>
        /// <param name="formatType">The item format type.</param>
        /// <param name="arguments">The arguments used to format the change message.</param>
        /// <returns>The formatted message that defines how the item has changed.</returns>
        string FormatItem(IItemDefinition definition, ItemFormatType formatType, IFormatArguments arguments);

        /// <summary>
        ///     Formats the specified item match using the provided arguments.
        /// </summary>
        /// <typeparam name="T">The type of item to format.</typeparam>
        /// <param name="match">The matching items to format.</param>
        /// <param name="formatType">The item format type.</param>
        /// <param name="arguments">The arguments used to format the change message.</param>
        /// <returns>The formatted message that defines how the item has changed.</returns>
        string FormatMatch<T>(ItemMatch<T> match, ItemFormatType formatType, IFormatArguments arguments)
            where T : IItemDefinition;
    }
}