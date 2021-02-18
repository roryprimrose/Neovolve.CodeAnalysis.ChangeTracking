namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;

    public static class TypeDefinitionExtensions
    {
        public static bool IsMatch(this ITypeDefinition oldType, ITypeDefinition newType)
        {
            oldType = oldType ?? throw new ArgumentNullException(nameof(oldType));
            newType = newType ?? throw new ArgumentNullException(nameof(newType));

            if (oldType.Namespace != newType.Namespace)
            {
                // Early exit if the namespace is different
                // No point running recursion to check if parent types match if the namespace is different
                return false;
            }

            if (oldType.DeclaringType != null
                && newType.DeclaringType == null)
            {
                // The old type has a parent type but the new one doesn't, no match
                return false;
            }

            if (oldType.DeclaringType == null
                && newType.DeclaringType != null)
            {
                // The new type has a parent type but the old one doesn't, no match
                return false;
            }

            // Check the parent types
            if (oldType.DeclaringType != null
                && newType.DeclaringType != null)
            {
                if (oldType.DeclaringType.IsMatch(newType.DeclaringType) == false)
                {
                    // The parent types don't match
                    return false;
                }
            }

            // At this point we either don't have parent types or the parent types match

            // Types are the same if they have the same name with the same number of generic type parameters
            // Check the number of generic type parameters first because if the number is different then it doesn't matter about the name
            // If it is a generic type then we need to parse the type parameters out to validate the name
            if (oldType.GenericTypeParameters.Count != newType.GenericTypeParameters.Count)
            {
                return false;
            }

            return oldType.RawName == newType.RawName;
        }
    }
}