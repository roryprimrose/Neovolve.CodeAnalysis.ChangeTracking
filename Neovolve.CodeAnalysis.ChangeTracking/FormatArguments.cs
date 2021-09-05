namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;

    public class FormatArguments
    {
        public const string DefaultPrefix = MessagePart.DefinitionType + " " + MessagePart.Identifier + " ";

        public FormatArguments(string messageFormat, string? oldValue, string? newValue)
        {
            MessageFormat = ApplyMessageFormatPrefix(messageFormat);
            OldValue = oldValue;
            NewValue = newValue;
        }

        private static string ApplyMessageFormatPrefix(string messageFormat)
        {
            messageFormat = messageFormat ?? throw new ArgumentNullException(nameof(messageFormat));

            if (messageFormat.Contains(MessagePart.DefinitionType))
            {
                return messageFormat;
            }

            if (messageFormat.Contains(MessagePart.Identifier))
            {
                return messageFormat;
            }

            // The message format doesn't contain either the definition type of identifier so we will add it via the default prefix
            var existingFormat = messageFormat.Trim();

            return DefaultPrefix + existingFormat;
        }

        public string MessageFormat { get; }
        public string? NewValue { get; }
        public string? OldValue { get; }
    }
}