namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public interface IFormatArguments
    {
        /// <summary>
        ///     Gets the message format used to calculate the message for a result.
        /// </summary>
        string MessageFormat { get; }

        /// <summary>
        ///     Gets the new value that may be merged into the message format.
        /// </summary>
        string? NewValue { get; }

        /// <summary>
        ///     Gets the old value that may be merged into the message format.
        /// </summary>
        string? OldValue { get; }
    }
}