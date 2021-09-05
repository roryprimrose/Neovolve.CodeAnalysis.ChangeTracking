namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class DefaultIdentifierFormatter : IIdentifierFormatter
    {
        public virtual string FormatIdentifier(IItemDefinition definition, ItemFormatType formatType)
        {
            definition = definition ?? throw new ArgumentNullException(nameof(definition));

            return FormatItemByType(definition, formatType);
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

            if (definition is IAttributeDefinition attribute)
            {
                // Format this as {attribute.Name} on {declaringElement}
                var attributeName = FormatItem(attribute, formatType);
                var declaringElement = FormatItemByType(attribute.DeclaringElement, ItemFormatType.None);

                return attributeName + " on " + declaringElement;
            }

            if (definition is IArgumentDefinition argument)
            {
                // Format the argument by its declaring item
                var argumentIdentifier = FormatItem(argument, formatType);
                var declaringElement = FormatItemByType(argument.DeclaringAttribute, ItemFormatType.None);

                return argumentIdentifier + " in " + declaringElement;
            }

            if (definition is IParameterDefinition parameter)
            {
                // Format this as {parameter.Name} in {declaringMember.FullName}
                var parameterName = FormatItem(parameter, formatType);
                var declaringMember = FormatItemByType(parameter.DeclaringMember, ItemFormatType.None);

                return parameterName + " in " + declaringMember;
            }

            // Format the item using the default logic
            return FormatItem(definition, formatType);
        }
    }
}