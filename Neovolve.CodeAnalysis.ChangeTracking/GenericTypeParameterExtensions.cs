namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class GenericTypeParameterExtensions
    {
        public static string GetMatchingGenericType(this IGenericTypeElement oldItem,
            string oldGenericType, IGenericTypeElement newItem)
        {
            var oldGenericTypes = oldItem.GenericTypeParameters.FastToList();
            var newGenericTypes = newItem.GenericTypeParameters.FastToList();

            var minGenericTypes = Math.Min(oldGenericTypes.Count, newGenericTypes.Count);

            if (minGenericTypes > 0)
            {
                var typeIndex = oldGenericTypes.IndexOf(oldGenericType);

                if (typeIndex >= 0)
                {
                    return newGenericTypes[typeIndex];
                }
            }

            ITypeDefinition? oldDeclaringType = null;
            ITypeDefinition? newDeclaringType = null;

            if (oldItem is ITypeDefinition oldType
                && newItem is ITypeDefinition newType)
            {
                oldDeclaringType = oldType.DeclaringType;
                newDeclaringType = newType.DeclaringType;
            }
            else if (oldItem is IMemberDefinition oldMember
                     && newItem is IMemberDefinition newMember)
            {
                oldDeclaringType = oldMember.DeclaringType;
                newDeclaringType = newMember.DeclaringType;
            }

            if (oldDeclaringType != null
                && newDeclaringType != null)
            {
                // Search the parents
                var mappedTypeName = GetMatchingGenericType(oldDeclaringType, oldGenericType, newDeclaringType);

                if (mappedTypeName != oldGenericType)
                {
                    // We have found the generic type parameter that has been renamed somewhere in the parent type hierarchy
                    return mappedTypeName;
                }
            }

            // We didn't find a generic type definition for the requested type so return the original value
            return oldGenericType;
        }
    }
}