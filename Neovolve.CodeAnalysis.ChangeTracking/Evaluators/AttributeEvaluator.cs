namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class AttributeEvaluator : Evaluator<IAttributeDefinition>, IAttributeEvaluator
    {
        protected override void FindMatches(IMatchAgent<IAttributeDefinition> agent)
        {
            agent.MatchOn(SameDefinition);
            agent.MatchOn(ChangedNamedArgument);
            agent.MatchOn(ChangedOrdinalArgument);
            agent.MatchOn(SameName);
        }

        private static bool ChangedNamedArgument(IAttributeDefinition oldItem, IAttributeDefinition newItem)
        {
            return EvaluateAttributes(oldItem, newItem, true, true);
        }

        private static bool ChangedOrdinalArgument(IAttributeDefinition oldItem, IAttributeDefinition newItem)
        {
            return EvaluateAttributes(oldItem, newItem, true);
        }

        private static bool EvaluateAttributes(
            IAttributeDefinition oldItem,
            IAttributeDefinition newItem,
            bool evaluateArgumentCounts = false,
            bool evaluateOrdinalArgumentValues = false,
            bool evaluateNamedArgumentValues = false)
        {
            if (oldItem.GetRawName() != newItem.GetRawName())
            {
                return false;
            }

            if (evaluateArgumentCounts && oldItem.Arguments.Count != newItem.Arguments.Count)
            {
                // There are a different number of arguments
                return false;
            }

            // Check each of the arguments
            var oldOrdinalArguments = oldItem.Arguments.Where(x => x.ArgumentType == ArgumentType.Ordinal).ToList();
            var newOrdinalArguments = newItem.Arguments.Where(x => x.ArgumentType == ArgumentType.Ordinal).ToList();

            if (evaluateArgumentCounts && oldOrdinalArguments.Count != newOrdinalArguments.Count)
            {
                return false;
            }

            if (evaluateOrdinalArgumentValues)
            {
                // Loop through all the ordinal parameters
                for (var index = 0; index < oldOrdinalArguments.Count; index++)
                {
                    var oldOrdinalArgument = oldOrdinalArguments[index];
                    var newOrdinalArgument = newOrdinalArguments[index];

                    if (oldOrdinalArgument.Value != newOrdinalArgument.Value)
                    {
                        // The ordinal argument doesn't match
                        return false;
                    }
                }
            }

            // There is no point evaluating named argument counts given that at this point there are the same number of arguments
            // and the same number of ordinal arguments. This means the number of named arguments must match
            
            if (evaluateNamedArgumentValues)
            {
                var oldNamedArguments = oldItem.Arguments.Where(x => x.ArgumentType == ArgumentType.Named).ToList();
                var newNamedArguments = newItem.Arguments.Where(x => x.ArgumentType == ArgumentType.Named).ToList();

                // Loop through all the named parameters
                foreach (var oldNamedArgument in oldNamedArguments)
                {
                    var argument = oldNamedArgument;
                    var newNamedArgument = newNamedArguments.FirstOrDefault(x => x.ParameterName == argument.ParameterName);

                    if (newNamedArgument == null)
                    {
                        // The old attribute has a parameter that the new attribute doesn't
                        return false;
                    }

                    if (oldNamedArgument.Value != newNamedArgument.Value)
                    {
                        // The named argument doesn't match
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool SameDefinition(IAttributeDefinition oldItem, IAttributeDefinition newItem)
        {
            return EvaluateAttributes(oldItem, newItem, true, true, true);
        }

        private static bool SameName(IAttributeDefinition oldItem, IAttributeDefinition newItem)
        {
            return EvaluateAttributes(oldItem, newItem);
        }
    }
}