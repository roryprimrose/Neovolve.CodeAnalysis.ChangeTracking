namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class ElementDefinitionExtensions
    {
        public static string GetDeclaredAccessModifiers(this IElementDefinition definition)
        {
            definition = definition ?? throw new ArgumentNullException(nameof(definition));

            var accessModifiers = new List<string>(2);
            var parts = definition.DeclaredModifiers.Split(new []{" " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                switch (part)
                {
                    case "private":
                    case "internal":
                    case "protected":
                    case "public":
                        accessModifiers.Add(part);
                        break;
                }
            }

            return string.Join(" ", accessModifiers);
        }

        public static string GetDeclaredModifiers(this IElementDefinition definition)
        {
            definition = definition ?? throw new ArgumentNullException(nameof(definition));

            var accessModifiers = new List<string>(2);
            var parts = definition.DeclaredModifiers.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                switch (part)
                {
                    case "private":
                    case "internal":
                    case "protected":
                    case "public":
                        break;
                    default:
                        accessModifiers.Add(part);
                        break;
                }
            }

            return string.Join(" ", accessModifiers);
        }
    }
}