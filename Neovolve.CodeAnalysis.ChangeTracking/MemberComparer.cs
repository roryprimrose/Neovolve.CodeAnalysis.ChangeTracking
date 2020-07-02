namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public abstract class MemberComparer<T> : ElementComparer<T>, IMemberComparer<T> where T : IMemberDefinition
    {
        protected MemberComparer(IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
        }

        protected override IEnumerable<ComparisonResult> EvaluateMatch(ItemMatch<T> match, ComparerOptions options)
        {
            if (match.OldItem.ReturnType != match.NewItem.ReturnType)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{match.OldItem.Description} return type has changed from {match.OldItem.ReturnType} {match.NewItem.ReturnType}");
            }
        }
    }
}