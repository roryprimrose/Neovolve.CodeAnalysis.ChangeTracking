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
            FormatArguments arguments)
        {
            definition = definition ?? throw new ArgumentNullException(nameof(definition));
            arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));

            var definitionType = FormatDefinitionType(definition, formatType);
            var identifier = FormatIdentifier(definition, formatType);
            var oldValue = FormatOldValue(definition, formatType, arguments.OldValue);
            var newValue = FormatNewValue(definition, formatType, arguments.NewValue);

            ValidateMessageMarkers(arguments);

            // The message format is expected to have markers for each of the above values
            var message = arguments.MessageFormat
#if NETSTANDARD2_1_OR_GREATER
                .Replace(MessagePart.DefinitionType, definitionType, StringComparison.Ordinal)
                .Replace(MessagePart.Identifier, identifier, StringComparison.Ordinal)
                .Replace(MessagePart.OldValue, oldValue, StringComparison.Ordinal)
                .Replace(MessagePart.NewValue, newValue, StringComparison.Ordinal);
#else
                .Replace(MessagePart.DefinitionType, definitionType)
                .Replace(MessagePart.Identifier, identifier)
                .Replace(MessagePart.OldValue, oldValue)
                .Replace(MessagePart.NewValue, newValue);
#endif

            return message;
        }

        public virtual string FormatMatch<T>(ItemMatch<T> match, ItemFormatType formatType, FormatArguments arguments)
            where T : IItemDefinition
        {
            return FormatItem(match.NewItem, formatType, arguments);
        }

        protected virtual string FormatDefinitionType(IItemDefinition definition, ItemFormatType formatType)
        {
            definition = definition ?? throw new ArgumentNullException(nameof(definition));

            if (definition is IClassDefinition)
            {
                return "Class";
            }

            if (definition is IInterfaceDefinition)
            {
                return "Interface";
            }

            if (definition is IEnumDefinition)
            {
                return "Enum";
            }

            if (definition is IEnumMemberDefinition)
            {
                return "Enum Member";
            }

            if (definition is IStructDefinition)
            {
                return "Struct";
            }

            if (definition is IConstraintListDefinition)
            {
                return "Generic constraint";
            }

            if (definition is IFieldDefinition)
            {
                return "Field";
            }

            if (definition is IConstructorDefinition)
            {
                return "Constructor";
            }

            if (definition is IMethodDefinition)
            {
                return "Method";
            }

            if (definition is IPropertyDefinition)
            {
                return "Property";
            }

            if (definition is IPropertyAccessorDefinition)
            {
                return "Property accessor";
            }

            if (definition is IParameterDefinition)
            {
                return "Parameter";
            }

            if (definition is IAttributeDefinition)
            {
                return "Attribute";
            }

            if (definition is IArgumentDefinition argument)
            {
                if (argument.ArgumentType == ArgumentType.Named)
                {
                    return "Named argument";
                }

                return "Ordinal argument";
            }

            return "Element";
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
        private static void ValidateMessageMarkers(FormatArguments arguments)
        {
            if (arguments.MessageFormat.Contains(MessagePart.DefinitionType) == false)
            {
                throw new InvalidOperationException(
                    "The message format arguments does not include " + MessagePart.DefinitionType);
            }

            if (string.IsNullOrWhiteSpace(arguments.Identifier) == false
                && arguments.MessageFormat.Contains(MessagePart.Identifier) == false)
            {
                throw new InvalidOperationException(
                    "The message format arguments provide an identifier but the message format does not include "
                    + MessagePart.Identifier);
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