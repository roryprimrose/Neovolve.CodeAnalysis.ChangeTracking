namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class EnumMemberEvaluator : Evaluator<IEnumMemberDefinition>, IEnumMemberEvaluator
    {
        protected override void FindMatches(IMatchAgent<IEnumMemberDefinition> agent)
        {
            agent.MatchOn(MemberName);
            agent.MatchOn(MemberValue);
            agent.MatchOn(MemberIndex);
        }

        private static bool MemberIndex(IEnumMemberDefinition oldItem, IEnumMemberDefinition newItem)
        {
            if (oldItem.Index != newItem.Index)
            {
                return false;
            }

            return true;
        }

        private static bool MemberName(IEnumMemberDefinition oldItem, IEnumMemberDefinition newItem)
        {
            if (oldItem.Name != newItem.Name)
            {
                return false;
            }

            return true;
        }

        private static bool MemberValue(IEnumMemberDefinition oldItem, IEnumMemberDefinition newItem)
        {
            if (string.IsNullOrWhiteSpace(oldItem.Value))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(newItem.Value))
            {
                return false;
            }

            if (oldItem.Value != newItem.Value)
            {
                return false;
            }

            return true;
        }
    }
}