namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class DefaultIdentifierFormatter : IIdentifierFormatter
    {
        public virtual string FormatIdentifier(IItemDefinition definition, ItemFormatType formatType)
        {
            definition = definition ?? throw new ArgumentNullException(nameof(definition));

            var message =  FormatItemByType(definition, formatType);

            // If the first character is a-z then make it upper case
            var firstCharacter = message[0];

            if (firstCharacter is >= 'a' and <= 'z')
            {
                var convertedCharacter = char.ToUpper(firstCharacter);

                message = convertedCharacter + message[1..];
            }

            return message;
        }

        protected virtual string FormatDefinitionType(IItemDefinition definition, ItemFormatType formatType)
        {
            definition = definition ?? throw new ArgumentNullException(nameof(definition));

            if (definition is IClassDefinition)
            {
                return "class";
            }

            if (definition is IInterfaceDefinition)
            {
                return "interface";
            }

            if (definition is IEnumDefinition)
            {
                return "enum";
            }

            if (definition is IEnumMemberDefinition)
            {
                return "enum member";
            }

            if (definition is IStructDefinition)
            {
                return "struct";
            }

            if (definition is IConstraintListDefinition)
            {
                return "generic constraint";
            }

            if (definition is IFieldDefinition)
            {
                return "field";
            }

            if (definition is IConstructorDefinition)
            {
                return "constructor";
            }

            if (definition is IMethodDefinition)
            {
                return "method";
            }

            if (definition is IPropertyDefinition)
            {
                return "property";
            }

            if (definition is IPropertyAccessorDefinition)
            {
                return "property accessor";
            }

            if (definition is IParameterDefinition)
            {
                return "parameter";
            }

            if (definition is IAttributeDefinition)
            {
                return "attribute";
            }

            if (definition is IArgumentDefinition argument)
            {
                if (argument.ArgumentType == ArgumentType.Named)
                {
                    return "named argument";
                }

                return "ordinal argument";
            }

            return "element";
        }

        protected virtual string FormatItem(IItemDefinition definition, ItemFormatType formatType)
        {
            definition = definition ?? throw new ArgumentNullException(nameof(definition));

            if (definition is IArgumentDefinition argument)
            {
                // Format the argument by its declaring item
                if (argument.ArgumentType == ArgumentType.Ordinal)
                {
                    var index = argument.OrdinalIndex ?? 0;

                    return (index + 1).ToString();
                }

                return argument.ParameterName;
            }

            if (definition is IParameterDefinition parameter)
            {
                return parameter.Type + " " + parameter.Name;
            }

            if (definition is IElementDefinition element)
            {
                return element.FullName;
            }

            return definition.Name;
        }

        protected virtual string FormatItemByType(IItemDefinition definition, ItemFormatType formatType)
        {
            definition = definition ?? throw new ArgumentNullException(nameof(definition));

            var definitionType = FormatDefinitionType(definition, formatType);

            if (definition is IAttributeDefinition attribute)
            {
                // Format this as {attribute.Name} on {declaringElement}
                var attributeName = FormatItem(attribute, formatType);
                var declaringElement = FormatItemByType(attribute.DeclaringElement, ItemFormatType.None);

                return definitionType + " " + attributeName + " on " + declaringElement;
            }

            if (definition is IArgumentDefinition argument)
            {
                // Format the argument by its declaring item
                var argumentIdentifier = FormatItem(argument, formatType);
                var declaringElement = FormatItemByType(argument.DeclaringAttribute, ItemFormatType.None);

                return definitionType + " " + argumentIdentifier + " in " + declaringElement;
            }

            if (definition is IParameterDefinition parameter)
            {
                // Format this as {parameter.Name} in {declaringMember.FullName}
                var parameterName = FormatItem(parameter, formatType);
                var declaringMember = FormatItemByType(parameter.DeclaringMember, ItemFormatType.None);

                return definitionType + " " + parameterName + " in " + declaringMember;
            }

            // Format the item using the default logic
            return definitionType + " " + FormatItem(definition, formatType);
        }
    }
}