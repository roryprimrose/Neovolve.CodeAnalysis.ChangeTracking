namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class TypeDefinitionExtensions
    {
        public static string GetFullNameWithoutGenericTypes(this ITypeDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (definition.GenericTypeParameters.Count == 0)
            {
                return definition.FullName;
            }

            return definition.FullName[..definition.FullName.IndexOf("<", StringComparison.Ordinal)];
        }
    }
}