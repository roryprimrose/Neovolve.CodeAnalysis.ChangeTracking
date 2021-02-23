namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using System;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TypeEvaluator : Evaluator<ITypeDefinition>, ITypeEvaluator
    {
        protected override void FindMatches(IMatchAgent<ITypeDefinition> agent)
        {
            agent.MatchOn(ExactSignature);
            agent.MatchOn((ITypeDefinition oldItem, ITypeDefinition newItem) => MovedType(agent, oldItem, newItem));
        }

        private static bool ExactSignature(ITypeDefinition oldType, ITypeDefinition newType)
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
                if (ExactSignature(oldType.DeclaringType, newType.DeclaringType) == false)
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

        private static bool MovedType(IMatchAgent<ITypeDefinition> agent, ITypeDefinition oldType,
            ITypeDefinition newType)
        {
            if (oldType.Name != newType.Name)
            {
                // The types don't have the same name
                return false;
            }

            if (oldType.GetType() != newType.GetType())
            {
                // The types don't have the same type
                return false;
            }

            // At this point the types should have different namespaces but we should double check just in case
            if (oldType.Namespace == newType.Namespace)
            {
                // It hasn't changed
                return false;
            }

            var possibleMatchesRemoved =
                agent.OldItems.Count(x => x.Name == oldType.Name);

            if (possibleMatchesRemoved > 1)
            {
                // There are multiple types with the same type and name in different namespaces
                // We therefore have to assume this isn't necessarily a change in namespace of a single class
                return false;
            }

            var possibleMatchesAdded =
                agent.NewItems.Count(x => x.Name == oldType.Name);

            if (possibleMatchesAdded > 1)
            {
                // There are multiple types with the same type and name in different namespaces
                // We therefore have to assume this isn't necessarily a change in namespace of a single class
                return false;
            }

            // We have a type that looks like it has changed namespace
            return true;
        }
    }
}