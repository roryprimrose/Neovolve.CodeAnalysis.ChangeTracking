namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;

    public class FormatArguments
    {
        public const string DefaultPrefix = MessagePart.DefinitionType + " " + MessagePart.Identifier + " ";

        public FormatArguments(string messageFormat, string identifier, string? oldValue, string? newValue)
        {
            messageFormat = messageFormat ?? throw new ArgumentNullException(nameof(messageFormat));
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            MessageFormat = ApplyMessageFormatPrefix(messageFormat);
            OldValue = oldValue;
            NewValue = newValue;
        }

        private static string ApplyMessageFormatPrefix(string format)
        {
            if (format.Contains(MessagePart.DefinitionType))
            {
                return format;
            }

            if (format.Contains(MessagePart.Identifier))
            {
                return format;
            }

            // The message format doesn't contain either the definition type of identifier so we will add it via the default prefix
            var existingFormat = format.Trim();

            return DefaultPrefix + existingFormat;
        }

        public string Identifier { get; }
        public string MessageFormat { get; }
        public string? NewValue { get; }
        public string? OldValue { get; }
    }
}