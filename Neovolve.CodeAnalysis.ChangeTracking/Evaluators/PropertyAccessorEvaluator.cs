namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyAccessorEvaluator : Evaluator<IPropertyAccessorDefinition>, IPropertyAccessorEvaluator
    {
        protected override void FindMatches(IMatchAgent<IPropertyAccessorDefinition> agent)
        {
            agent.MatchOn(PropertyAccessorName);
        }

        private static bool PropertyAccessorName(IPropertyAccessorDefinition oldItem, IPropertyAccessorDefinition newItem)
        {
            // Take a match on the exact name first
            if (oldItem.Name == newItem.Name)
            {
                return true;
            }

            // Next accept a match on the read or write purpose of the accessor
            // This captures switches between set and init accessors
            if (oldItem.AccessorPurpose == newItem.AccessorPurpose)
            {
                return true;
            }

            // This must be comparing a get to either an init or a set
            return false;

        }
    }
}