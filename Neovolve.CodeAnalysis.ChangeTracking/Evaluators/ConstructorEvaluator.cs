﻿namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using System.Collections.Generic;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class ConstructorEvaluator : Evaluator<IConstructorDefinition>, IConstructorEvaluator
    {
        public override IEvaluationResults<IConstructorDefinition> FindMatches(IEnumerable<IConstructorDefinition> oldItems,
            IEnumerable<IConstructorDefinition> newItems)
        {
            var oldConstructors = oldItems.ToList();
            var newConstructors = newItems.ToList();

            var matches = base.FindMatches(oldConstructors, newConstructors);

            // We need to look for default constructors (instance or static) that have been added or removed where there is no other constructor
            if (oldConstructors.Count + newConstructors.Count != 1)
            {
                return matches;
            }

            // There is a single constructor that is either added or removed

            // First we need to find the constructor
            var constructor = oldConstructors.Count > 0 ? oldConstructors[0] : newConstructors[0];

            // Next validate whether it is a default constructor
            if (constructor.Parameters.Count > 0)
            {
                // It isn't a default constructor so we need to return the existing matches
                // which would identify the constructor as either added or removed
                return matches;
            }

            // If we got this far then a default constructor has been added or removed and there are no other constructors
            // We need to wipe out the match results which would have identified this as a feature (added) or breaking (removed) change
            return EvaluationResults<IConstructorDefinition>.Empty;
        }

        protected override void FindMatches(IMatchAgent<IConstructorDefinition> agent)
        {
            agent.MatchOn(ExactSignature);
            agent.MatchOn(ChangedModifiers);
            agent.MatchOn(ChangedParameterTypes);
            agent.MatchOn(ChangedParameterCount);
        }

        private static bool ChangedModifiers(IConstructorDefinition newItem, IConstructorDefinition oldItem)
        {
            // Check for a change of the modifiers
            // This can only be a change to or from a static constructor in which case there must be no parameters
            if (newItem.Parameters.Count > 0)
            {
                return false;
            }

            if (oldItem.Parameters.Count > 0)
            {
                return false;
            }

            // The constructors have no parameters but have changed modifiers (because ExactMatch would have already matched to a constructor with the same modifier)
            return true;
        }

        private static bool ChangedParameterCount(IConstructorDefinition newItem, IConstructorDefinition oldItem)
        {
            if (newItem.Modifiers != oldItem.Modifiers)
            {
                return false;
            }

            // At this point we have ruled out exact matches, changes to modifiers and changes to parameter types
            // The only other possible combination is that the parameter counts have changed
            // We will call this a match as constructors can't match on names
            // We will rely on IMatchAgent to detect whether there is only one other constructor to match this condition to truely identify the match
            return true;
        }

        private static bool ChangedParameterTypes(IConstructorDefinition newItem, IConstructorDefinition oldItem)
        {
            // Find all the constructors that match by modifiers and parameter count
            if (newItem.Modifiers != oldItem.Modifiers)
            {
                return false;
            }

            if (newItem.Parameters.Count != oldItem.Parameters.Count)
            {
                return false;
            }

            // The methods have the same modifiers and parameter count
            // Given that ExactMatch has already been evaluated then there must be a change of a parameter type
            return true;
        }

        private static bool ExactSignature(IConstructorDefinition newItem, IConstructorDefinition oldItem)
        {
            // Find all the constructors that match by modifiers, parameter count and parameter types
            // This is finding direct matches on constructors signatures
            if (newItem.Modifiers != oldItem.Modifiers)
            {
                return false;
            }

            if (newItem.Parameters.Count != oldItem.Parameters.Count)
            {
                return false;
            }

            if (ParameterListMatches(oldItem, newItem) == false)
            {
                return false;
            }

            // The constructors have the modifier, parameter count and parameter types
            return true;
        }

        private static bool ParameterListMatches(IConstructorDefinition oldConstructor,
            IConstructorDefinition newConstructor)
        {
            var oldParameters = oldConstructor.Parameters.ToList();
            var newParameters = newConstructor.Parameters.ToList();

            for (var index = 0; index < oldParameters.Count; index++)
            {
                var oldParameter = oldParameters[index];
                var newParameter = newParameters[index];
                var oldType = oldParameter.Type;
                var newType = newParameter.Type;

                var oldMappedType =
                    oldConstructor.DeclaringType.GetMatchingGenericType(oldType, newConstructor.DeclaringType);

                if (oldMappedType != newType)
                {
                    return false;
                }
            }

            return true;
        }
    }
}