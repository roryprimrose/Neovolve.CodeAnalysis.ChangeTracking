namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Diagnostics;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class DefaultMessageFormatter : IMessageFormatter
    {
        public virtual string FormatItemAddedMessage(IItemDefinition definition, FormatArguments arguments)
        {
            definition = definition ?? throw new ArgumentNullException(nameof(definition));
            arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));

            var definitionType = FormatDefinitionType(definition);
            var identifier = FormatItemAddedIdentifier(definition, arguments.Identifier);
            var oldValue = FormatOldValue(definition, arguments.OldValue);
            var newValue = FormatNewValue(definition, arguments.NewValue);

            ValidateMessageMarkers(arguments);

            // The message format is expected to have markers for each of the above values
            var message = arguments.MessageFormat
                .Replace("{DefinitionType}", definitionType)
                .Replace("{Identifier}", identifier)
                .Replace("{OldValue}", oldValue)
                .Replace("{NewValue}", newValue);

            return message;
        }

        public virtual string FormatItemChangedMessage<T>(ItemMatch<T> match, FormatArguments arguments)
            where T : IItemDefinition
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));

            var definitionType = FormatDefinitionType(match);
            var identifier = FormatIdentifier(match, arguments.Identifier);
            var oldValue = FormatOldValue(match, arguments.OldValue);
            var newValue = FormatNewValue(match, arguments.NewValue);

            ValidateMessageMarkers(arguments);

            // The message format is expected to have markers for each of the above values
            var message = arguments.MessageFormat
                .Replace("{DefinitionType}", definitionType)
                .Replace("{Identifier}", identifier)
                .Replace("{OldValue}", oldValue)
                .Replace("{NewValue}", newValue);

            return message;
        }

        public virtual string FormatItemRemovedMessage(IItemDefinition definition, FormatArguments arguments)
        {
            definition = definition ?? throw new ArgumentNullException(nameof(definition));
            arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));

            var definitionType = FormatDefinitionType(definition);
            var identifier = FormatItemRemovedIdentifier(definition, arguments.Identifier);
            var oldValue = FormatOldValue(definition, arguments.OldValue);
            var newValue = FormatNewValue(definition, arguments.NewValue);

            ValidateMessageMarkers(arguments);

            // The message format is expected to have markers for each of the above values
            var message = arguments.MessageFormat
                .Replace("{DefinitionType}", definitionType)
                .Replace("{Identifier}", identifier)
                .Replace("{OldValue}", oldValue)
                .Replace("{NewValue}", newValue);

            return message;
        }

        protected virtual string FormatDefinitionType<T>(ItemMatch<T> match) where T : IItemDefinition
        {
            return FormatDefinitionType(match.OldItem);
        }

        protected virtual string FormatDefinitionType(IItemDefinition definition)
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

        protected virtual string FormatIdentifier<T>(ItemMatch<T> match, string identifier) where T : IItemDefinition
        {
            return FormatIdentifier(match.OldItem, identifier);
        }

        protected virtual string FormatItemAddedIdentifier(IItemDefinition definition, string identifier)
        {
            return FormatIdentifier(definition, identifier);
        }

        protected virtual string FormatItemRemovedIdentifier(IItemDefinition definition, string identifier)
        {
            return FormatIdentifier(definition, identifier);
        }

        protected virtual string FormatIdentifier(IItemDefinition definition, string identifier)
        {
            return identifier;
        }

        protected virtual string FormatNewValue(IItemDefinition definition, string? value)
        {
            return value ?? string.Empty;
        }

        protected virtual string FormatOldValue(IItemDefinition definition, string? value)
        {
            return value ?? string.Empty;
        }

        [Conditional("DEBUG")]
        private static void ValidateMessageMarkers(FormatArguments arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments.Identifier) == false
                && arguments.MessageFormat.Contains("{Identifier}") == false)
            {
                throw new InvalidOperationException(
                    "The message format arguments provide an identifier but the message format does not include {Identifier}");
            }

            if (string.IsNullOrWhiteSpace(arguments.OldValue) == false
                && arguments.MessageFormat.Contains("{OldValue}") == false)
            {
                throw new InvalidOperationException(
                    "The message format arguments provide an old value but the message format does not include {OldValue}");
            }

            if (string.IsNullOrWhiteSpace(arguments.NewValue) == false
                && arguments.MessageFormat.Contains("{NewValue}") == false)
            {
                throw new InvalidOperationException(
                    "The message format arguments provide a new value but the message format does not include {NewValue}");
            }
        }

        private string FormatNewValue<T>(ItemMatch<T> match, string? value) where T : IItemDefinition
        {
            return FormatNewValue(match.OldItem, value);
        }

        private string FormatOldValue<T>(ItemMatch<T> match, string? value) where T : IItemDefinition
        {
            return FormatOldValue(match.OldItem, value);
        }
    }
}
