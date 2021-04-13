namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyEvaluator : Evaluator<IPropertyDefinition>, IPropertyEvaluator
    {
        protected override void FindMatches(IMatchAgent<IPropertyDefinition> agent)
        {
            agent.MatchOn(PropertyName);
        }

        private static bool PropertyName(IPropertyDefinition oldItem, IPropertyDefinition newItem)
        {
            if (oldItem.Name != newItem.Name)
            {
                return false;
            }

            return true;
        }
    }
}