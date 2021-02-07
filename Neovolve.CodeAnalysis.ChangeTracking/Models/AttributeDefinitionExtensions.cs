namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class AttributeDefinitionExtensions
    {
        public static string GetRawName(this AttributeSyntax node)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            return GetRawName(node.Name.ToString());
        }

        public static string GetRawName(this IAttributeDefinition attribute)
        {
            attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));

            return GetRawName(attribute.Name);
        }

        private static string GetRawName(string name)
        {
            // This assumes that the expressions in ComparerOptions do not handle the Attribute suffix that is not required by the compiler
            if (name.EndsWith("Attribute", StringComparison.OrdinalIgnoreCase))
            {
                return name.Substring(0, name.Length - 9);
            }

            return name;
        }
    }
}