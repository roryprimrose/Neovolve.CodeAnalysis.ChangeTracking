namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class DefaultMessageFormatter : IMessageFormatter
    {
        public string FormatMessage(IItemDefinition definition, FormatArguments arguments)
        {
            definition = definition ?? throw new ArgumentNullException(nameof(definition));
            arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));

            var definitionType = FormatDefinitionType(definition);
            var identifier = FormatIdentifier(definition, arguments.Identifier);
            var oldValue = FormatOldValue(definition, arguments.OldValue);
            var newValue = FormatNewValue(definition, arguments.NewValue);

            // The message format is expected to have markers for each of the above values
            var message = arguments.MessageFormat
                .Replace("{DefinitionType}", definitionType)
                .Replace("{Identifier}", identifier)
                .Replace("{OldValue}", oldValue)
                .Replace("{NewValue}", newValue);

            return message;
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

            if (definition is IConstraintListDefinition)
            {
                return "Generic constraint";
            }

            if (definition is IPropertyDefinition)
            {
                return "Property";
            }

            if (definition is IPropertyAccessorDefinition)
            {
                return "Property accessor";
            }

            if (definition is IFieldDefinition)
            {
                return "Field";
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
    }
}