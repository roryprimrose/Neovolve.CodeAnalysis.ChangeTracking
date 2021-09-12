namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;

    /// <summary>
    ///     The <see cref="FormatArguments" />
    ///     class is used to identify the message format and possible change values used to build a result message.
    /// </summary>
    /// <remarks>
    ///     This class will default <see cref="MessageFormat" /> to use <see cref="DefaultPrefix" /> if
    ///     <see cref="MessagePart.Identifier" /> is not found in the format
    ///     value.
    /// </remarks>
    public class FormatArguments : IFormatArguments
    {
        public const string DefaultPrefix = MessagePart.Identifier + " ";

        /// <summary>
        ///     Initializes a new instance of the <see cref="FormatArguments" /> class.
        /// </summary>
        /// <param name="messageFormat">The message format used to calculate the message for a result.</param>
        /// <param name="oldValue">The new value that may be merged into the message format.</param>
        /// <param name="newValue">The old value that may be merged into the message format.</param>
        public FormatArguments(string messageFormat, string? oldValue, string? newValue)
        {
            MessageFormat = ApplyMessageFormatPrefix(messageFormat);
            OldValue = oldValue;
            NewValue = newValue;
        }

        private static string ApplyMessageFormatPrefix(string messageFormat)
        {
            messageFormat = messageFormat ?? throw new ArgumentNullException(nameof(messageFormat));
            
            if (messageFormat.Contains(MessagePart.Identifier))
            {
                return messageFormat;
            }

            // The message format doesn't contain either the definition type of identifier so we will add it via the default prefix
            var existingFormat = messageFormat.Trim();

            return DefaultPrefix + existingFormat;
        }

        /// <inheritdoc />
        public string MessageFormat { get; }

        /// <inheritdoc />
        public string? NewValue { get; }

        /// <inheritdoc />
        public string? OldValue { get; }
    }
}