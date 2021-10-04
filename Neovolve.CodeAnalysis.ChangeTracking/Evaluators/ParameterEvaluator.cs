namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class ParameterEvaluator : Evaluator<IParameterDefinition>, IParameterEvaluator
    {
        protected override void FindMatches(IMatchAgent<IParameterDefinition> agent)
        {
            agent.MatchOn(ParameterName);
            agent.MatchOn(ParameterIndex);
        }

        private static bool ParameterIndex(IParameterDefinition oldItem, IParameterDefinition newItem)
        {
            if (oldItem.DeclaredIndex != newItem.DeclaredIndex)
            {
                return false;
            }

            return true;
        }

        private static bool ParameterName(IParameterDefinition oldItem, IParameterDefinition newItem)
        {
            if (oldItem.Name != newItem.Name)
            {
                return false;
            }

            return true;
        }
    }
}