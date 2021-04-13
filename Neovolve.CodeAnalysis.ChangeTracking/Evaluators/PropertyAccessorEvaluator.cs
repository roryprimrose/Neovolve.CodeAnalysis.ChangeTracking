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
            if (oldItem.Name != newItem.Name)
            {
                return false;
            }

            return true;
        }
    }
}