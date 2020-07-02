namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class MemberDefinitionExtensions
    {
        public static bool ReturnTypeIsGeneric(this IMemberDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            var returnType = definition.ReturnType;

            if (definition.DeclaringType == null)
            {
                return false;
            }

            if (definition.DeclaringType.GenericTypeParameters.Count == 0)
            {
                return false;
            }

            if (definition.DeclaringType.GenericTypeParameters.Contains(returnType))
            {
                return true;
            }

            return false;
        }
    }
}