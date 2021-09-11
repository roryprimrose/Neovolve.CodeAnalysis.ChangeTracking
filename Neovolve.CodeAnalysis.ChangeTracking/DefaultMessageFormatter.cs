namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Diagnostics;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class DefaultMessageFormatter : IMessageFormatter
    {
        private readonly IIdentifierFormatter _identifierFormatter;

        public DefaultMessageFormatter(IIdentifierFormatter identifierFormatter)
        {
            _identifierFormatter = identifierFormatter ?? throw new ArgumentNullException(nameof(identifierFormatter));
        }

        public virtual string FormatItem(IItemDefinition definition, ItemFormatType formatType,
            IFormatArguments arguments)
        {
            definition = definition ?? throw new ArgumentNullException(nameof(definition));
            arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));

            var identifier = FormatIdentifier(definition, formatType);
            var oldValue = FormatOldValue(definition, formatType, arguments.OldValue);
            var newValue = FormatNewValue(definition, formatType, arguments.NewValue);

            ValidateMessageMarkers(arguments);

            // The message format is expected to have markers for each of the above values
            var message = arguments.MessageFormat
                .Replace(MessagePart.Identifier, identifier, StringComparison.Ordinal)
                .Replace(MessagePart.OldValue, oldValue, StringComparison.Ordinal)
                .Replace(MessagePart.NewValue, newValue, StringComparison.Ordinal);

            // If the first character is a-z then make it upper case
            var firstCharacter = message[0];

            if (firstCharacter is >= 'a' and <= 'z')
            {
                var convertedCharacter = char.ToUpper(firstCharacter);

                message = convertedCharacter + message[1..];
            }

            return message;
        }

        public virtual string FormatMatch<T>(ItemMatch<T> match, ItemFormatType formatType, IFormatArguments arguments)
            where T : IItemDefinition
        {
            return FormatItem(match.NewItem, formatType, arguments);
        }

        protected virtual string FormatIdentifier(IItemDefinition definition, ItemFormatType formatType)
        {
            return _identifierFormatter.FormatIdentifier(definition, formatType);
        }

        protected virtual string FormatNewValue(IItemDefinition definition, ItemFormatType formatType, string? value)
        {
            return value ?? string.Empty;
        }

        protected virtual string FormatOldValue(IItemDefinition definition, ItemFormatType formatType, string? value)
        {
            return value ?? string.Empty;
        }

        [Conditional("DEBUG")]
        private static void ValidateMessageMarkers(IFormatArguments arguments)
        {
            if (arguments.MessageFormat.Contains(MessagePart.Identifier) == false)
            {
                throw new InvalidOperationException(
                    "The message format arguments does not include " + MessagePart.Identifier);
            }

            if (string.IsNullOrWhiteSpace(arguments.OldValue) == false
                && arguments.MessageFormat.Contains(MessagePart.OldValue) == false)
            {
                throw new InvalidOperationException(
                    "The message format arguments provide an old value but the message format does not include "
                    + MessagePart.OldValue);
            }

            if (string.IsNullOrWhiteSpace(arguments.NewValue) == false
                && arguments.MessageFormat.Contains(MessagePart.NewValue) == false)
            {
                throw new InvalidOperationException(
                    "The message format arguments provide a new value but the message format does not include "
                    + MessagePart.NewValue);
            }
        }
    }
}