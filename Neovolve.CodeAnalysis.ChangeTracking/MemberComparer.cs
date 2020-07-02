namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using System.Linq;
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
                var genericTypeMatch = MapGenericTypeName(match);

                if (genericTypeMatch != match.NewItem.ReturnType)
                {
                    yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                        $"{match.OldItem.Description} return type has changed from {match.OldItem.ReturnType} {match.NewItem.ReturnType}");
                }
            }
        }

        public string MapGenericTypeName(ItemMatch<T> match)
        {
            var typeName = match.OldItem.ReturnType;

            if (match.OldItem.DeclaringType == null)
            {
                return typeName;
            }

            if (match.NewItem.DeclaringType == null)
            {
                return typeName;
            }

            var oldGenericTypes = match.OldItem.DeclaringType.GenericTypeParameters.ToList();

            if (oldGenericTypes.Count == 0)
            {
                return typeName;
            }

            var newGenericTypes = match.NewItem.DeclaringType.GenericTypeParameters.ToList();
            var typeIndex = newGenericTypes.IndexOf(typeName);

            if (typeIndex == -1)
            {
                return typeName;
            }

            return newGenericTypes[typeIndex];
        }
    }
}