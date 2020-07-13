namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public abstract class MemberComparer<T> : ElementComparer<T>, IMemberComparer<T> where T : IMemberDefinition
    {
        protected MemberComparer(IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
        }

        protected override void EvaluateMatch(
            ItemMatch<T> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            RunComparisonStep(EvaluateAccessModifierChanges, match, options, aggregator);
            RunComparisonStep(EvaluateReturnTypeChanges, match, options, aggregator);
        }

        private static void EvaluateAccessModifierChanges(
            ItemMatch<T> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            var change = AccessModifierChangeTable.CalculateChange(match);

            if (change == SemVerChangeType.None)
            {
                return;
            }

            if (match.OldItem.AccessModifier == AccessModifier.None)
            {
                // Modifiers have been added where there were previously none defined
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has added the access modifiers {match.NewItem.AccessModifier}");

                aggregator.AddResult(result);
            }
            else if (match.NewItem.AccessModifier == AccessModifier.None)
            {
                // All previous modifiers have been removed
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has removed the access modifiers {match.OldItem.AccessModifier}");

                aggregator.AddResult(result);
            }
            else
            {
                // Modifiers have been changed
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has changed access modifiers from {match.OldItem.AccessModifier} to {match.NewItem.AccessModifier}");

                aggregator.AddResult(result);
            }
        }

        private static string MapGenericTypeName(ItemMatch<T> match)
        {
            var typeName = match.OldItem.ReturnType;

            // We need to determine all the generic type parameters from the complete parent hierarchy not just the parent type

            var oldDeclaringType = match.OldItem.DeclaringType;
            var newDeclaringType = match.NewItem.DeclaringType;

            return ResolveRenamedGenericTypeParameter(typeName, oldDeclaringType, newDeclaringType);
        }

        private static string ResolveRenamedGenericTypeParameter(
            string originalTypeName,
            ITypeDefinition oldDeclaringType,
            ITypeDefinition newDeclaringType)
        {
            if (oldDeclaringType.DeclaringType != null
                && newDeclaringType.DeclaringType != null)
            {
                // Search the parents
                var mappedTypeName = ResolveRenamedGenericTypeParameter(
                    originalTypeName,
                    oldDeclaringType.DeclaringType,
                    newDeclaringType.DeclaringType);

                if (mappedTypeName != originalTypeName)
                {
                    // We have found the generic type parameter that has been renamed somewhere in the parent type hierarchy
                    return mappedTypeName;
                }
            }

            var oldGenericTypes = oldDeclaringType.GenericTypeParameters.ToList();

            if (oldGenericTypes.Count == 0)
            {
                return originalTypeName;
            }

            var newGenericTypes = newDeclaringType.GenericTypeParameters.ToList();
            var typeIndex = oldGenericTypes.IndexOf(originalTypeName);

            if (typeIndex == -1)
            {
                return originalTypeName;
            }

            return newGenericTypes[typeIndex];
        }

        private void EvaluateReturnTypeChanges(ItemMatch<T> match, ComparerOptions options, ChangeResultAggregator aggregator)
        {
            if (match.OldItem.ReturnType != match.NewItem.ReturnType)
            {
                var genericTypeMatch = MapGenericTypeName(match);

                if (genericTypeMatch != match.NewItem.ReturnType)
                {
                    var result = ComparisonResult.ItemChanged(
                        SemVerChangeType.Breaking,
                        match,
                        $"{match.NewItem.Description} return type has changed from {match.OldItem.ReturnType} {match.NewItem.ReturnType}");

                    aggregator.AddResult(result);
                }
            }
        }
    }
}