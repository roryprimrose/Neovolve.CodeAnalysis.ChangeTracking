namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class MethodEvaluator : Evaluator<IMethodDefinition>, IMethodEvaluator
    {
        protected override void FindMatches(IMatchAgent<IMethodDefinition> agent)
        {
            agent.MatchOn(ExactSignature);
            agent.MatchOn(ChangedReturnType);
            agent.MatchOn(ChangedName);
            agent.MatchOn(ChangedParameterTypes);
            agent.MatchOn(ChangedParameterCount);
            agent.MatchOn(ChangeGenericTypeParametersCount);

            // At this point we have matched as many related methods between the old code and the new code
        }

        private static bool ChangedName(IMethodDefinition newItem, IMethodDefinition oldItem)
        {
            // Find members that have been renamed where there is only one where the parameter set is the same but with a different name
            if (oldItem.ReturnType != newItem.ReturnType)
            {
                return false;
            }

            if (newItem.GenericTypeParameters.Count != oldItem.GenericTypeParameters.Count)
            {
                return false;
            }

            if (ParameterListMatches(oldItem, newItem) == false)
            {
                return false;
            }

            return true;
        }

        private static bool ChangedParameterCount(IMethodDefinition newItem, IMethodDefinition oldItem)
        {
            // Find members with the same name and generic type parameter count but different parameters
            // In this case it is a member that has changed the parameters
            if (newItem.RawName != oldItem.RawName)
            {
                return false;
            }

            if (oldItem.ReturnType != newItem.ReturnType)
            {
                return false;
            }

            if (newItem.GenericTypeParameters.Count != oldItem.GenericTypeParameters.Count)
            {
                return false;
            }

            // The methods have the same name and generic type parameters
            return true;
        }
        
        private static bool ChangedParameterTypes(IMethodDefinition newItem, IMethodDefinition oldItem)
        {
            // Find all members that match on name, generic type parameter count and parameter count
            // This is finding methods that have changed their parameter types
            if (newItem.RawName != oldItem.RawName)
            {
                return false;
            }

            if (newItem.GenericTypeParameters.Count != oldItem.GenericTypeParameters.Count)
            {
                return false;
            }

            if (newItem.Parameters.Count != oldItem.Parameters.Count)
            {
                return false;
            }

            // The methods have the same name, generic type parameter count and parameter count
            return true;
        }

        private static bool ChangedReturnType(IMethodDefinition newItem, IMethodDefinition oldItem)
        {
            // Find all the members that match by name, generic type parameter count and parameter set
            // This is finding direct matches on method signatures
            if (newItem.RawName != oldItem.RawName)
            {
                return false;
            }

            if (newItem.GenericTypeParameters.Count != oldItem.GenericTypeParameters.Count)
            {
                return false;
            }

            if (ParameterListMatches(oldItem, newItem) == false)
            {
                return false;
            }

            // The methods have the same name, generic type parameter count, parameter count and parameter types
            return true;
        }

        private static bool ChangeGenericTypeParametersCount(IMethodDefinition newItem, IMethodDefinition oldItem)
        {
            // Find all members that match on name and parameter count but generic type parameter count has changed
            // This is finding methods that have changed their generic type parameters
            if (newItem.RawName != oldItem.RawName)
            {
                return false;
            }

            if (newItem.Parameters.Count != oldItem.Parameters.Count)
            {
                return false;
            }

            // The methods have the same name and parameter count
            return true;
        }

        private static bool ExactSignature(IMethodDefinition newItem, IMethodDefinition oldItem)
        {
            // Find all the members that match by name, return type, generic type parameter count and parameter set
            // This is finding direct matches on method signatures
            if (newItem.RawName != oldItem.RawName)
            {
                return false;
            }

            if (oldItem.ReturnType != newItem.ReturnType)
            {
                return false;
            }

            if (newItem.GenericTypeParameters.Count != oldItem.GenericTypeParameters.Count)
            {
                return false;
            }

            if (ParameterListMatches(oldItem, newItem) == false)
            {
                return false;
            }

            // The methods have the same name, return type, generic type parameter count, parameter count and parameter types
            return true;
        }

        private static bool ParameterListMatches(IMethodDefinition oldMethod, IMethodDefinition newMethod)
        {
            if (oldMethod.Parameters.Count != newMethod.Parameters.Count)
            {
                return false;
            }

            var oldParameters = oldMethod.Parameters.ToList();
            var newParameters = newMethod.Parameters.ToList();

            for (var index = 0; index < oldParameters.Count; index++)
            {
                var oldParameter = oldParameters[index];
                var newParameter = newParameters[index];
                var oldType = oldParameter.Type;
                var newType = newParameter.Type;

                var oldMappedType =
                    oldMethod.GetMatchingGenericType(oldType, newMethod);
                
                if (oldMappedType != newType)
                {
                    return false;
                }
            }

            return true;
        }
    }
}