namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TypeEvaluator : Evaluator<ITypeDefinition>, ITypeEvaluator
    {
        protected override void FindMatches(IMatchAgent<ITypeDefinition> agent)
        {
            agent.MatchOn(ExactSignature);
            agent.MatchOn(DifferentGenericTypes);
            agent.MatchOn(MovedType);
            agent.MatchOn(ChangedTypeDefinition);
        }

        private static bool ExactSignature(ITypeDefinition oldType, ITypeDefinition newType)
        {
            return IsSameType(oldType, newType);
        }

        private static bool ChangedTypeDefinition(ITypeDefinition oldType, ITypeDefinition newType)
        {
            return IsSameType(oldType, newType, false);
        }

        private static bool DifferentGenericTypes(ITypeDefinition oldType, ITypeDefinition newType)
        {
            return IsSameType(oldType, newType, true, true, false);
        }

        private static bool IsSameType(ITypeDefinition oldType, ITypeDefinition newType,
            bool evaluateTypeDefinition = true, bool evaluateNamespace = true, bool evaluateGenericTypes = true)
        {
            oldType = oldType ?? throw new ArgumentNullException(nameof(oldType));
            newType = newType ?? throw new ArgumentNullException(nameof(newType));

            if (evaluateTypeDefinition && oldType.GetType() != newType.GetType())
            {
                // The types are different (for example one is a class and the other is an interface)
                return false;
            }

            if (evaluateNamespace && oldType.Namespace != newType.Namespace)
            {
                // Early exit if the namespace is different
                // No point running recursion to check if parent types match if the namespace is different
                return false;
            }

            if (oldType.RawName != newType.RawName)
            {
                // The names of the types are different
                return false;
            }

            // Types are the same if they have the same name with the same number of generic type parameters
            // Check the number of generic type parameters first because if the number is different then it doesn't matter about the name
            // If it is a generic type then we need to parse the type parameters out to validate the name
            if (evaluateGenericTypes && oldType.GenericTypeParameters.Count != newType.GenericTypeParameters.Count)
            {
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
                if (IsSameType(oldType.DeclaringType, newType.DeclaringType, evaluateTypeDefinition, evaluateNamespace,
                    evaluateGenericTypes) == false)
                {
                    // The parent types don't match
                    return false;
                }
            }

            // At this point we either don't have parent types or the parent types match

            return true;
        }

        private static bool MovedType(ITypeDefinition oldType, ITypeDefinition newType)
        {
            return IsSameType(oldType, newType, true, false);
        }
    }
}