namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class AttributeEvaluator : Evaluator<IAttributeDefinition>, IAttributeEvaluator
    {
        protected override void FindMatches(IMatchAgent<IAttributeDefinition> agent)
        {
            agent.MatchOn(SameName);
        }

        private static bool SameName(IAttributeDefinition oldItem, IAttributeDefinition newItem)
        {
            return oldItem.GetRawName() == newItem.GetRawName();
        }
    }
}