namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class FieldEvaluator : Evaluator<IFieldDefinition>, IFieldEvaluator
    {
        protected override void FindMatches(IMatchAgent<IFieldDefinition> agent)
        {
            agent.MatchOn(FieldName);
        }

        private static bool FieldName(IFieldDefinition oldItem, IFieldDefinition newItem)
        {
            if (oldItem.Name != newItem.Name)
            {
                return false;
            }

            return true;
        }
    }
}