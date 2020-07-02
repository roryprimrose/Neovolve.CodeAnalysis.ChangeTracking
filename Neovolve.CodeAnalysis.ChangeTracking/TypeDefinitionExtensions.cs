namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class TypeDefinitionExtensions
    {
        public static string GetNameWithoutGenericTypes(this ITypeDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (definition.GenericTypeParameters.Count == 0)
            {
                return definition.Name;
            }

            return definition.Name[..definition.Name.IndexOf("<", StringComparison.Ordinal)];
        }

        public static bool IsMatch(this ITypeDefinition oldType, ITypeDefinition newType)
        {
            // Check the parent types
            if (oldType.DeclaringType == null
                && newType.DeclaringType == null)
            {
                // There are no parent types
                return oldType.FullName == newType.FullName;
            }
            
            if (newType.DeclaringType == null)
            {
                // The old type has a parent type but the new one doesn't, no match
                return false;
            }

            if (oldType.DeclaringType == null)
            {
                // The old type has a parent type but the new one doesn't, no match
                return false;
            }

            // At this point both the new and old types have parent types
            // Recursively check the parents first
            if (oldType.DeclaringType.IsMatch(newType.DeclaringType) == false)
            {
                // The parents don't match so the current ones can't
                return false;
            }

            // Types are the same if they have the same name with the same number of generic type parameters
            // Check the number of generic type parameters first because if the number is different then it doesn't matter about the name
            // If it is a generic type then we need to parse the type parameters out to validate the name
            if (oldType.GenericTypeParameters.Count != newType.GenericTypeParameters.Count)
            {
                return false;
            }

            if (oldType.GenericTypeParameters.Count == 0)
            {
                return oldType.Name == newType.Name;
            }

            // Both the types are generic types
            var oldName = oldType.GetNameWithoutGenericTypes();
            var newName = newType.GetNameWithoutGenericTypes();

            return oldName == newName;
        }
    }
}