namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public abstract class MemberComparer<T> : ElementComparer<T>, IMemberComparer<T> where T : IMemberDefinition
    {
        private readonly IAccessModifiersComparer _accessModifiersComparer;
        
        protected MemberComparer(
            IAccessModifiersComparer accessModifiersComparer, IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
            _accessModifiersComparer = accessModifiersComparer
                                       ?? throw new ArgumentNullException(nameof(accessModifiersComparer));
        }

        protected override void EvaluateMatch(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            RunComparisonStep(EvaluateAccessModifierChanges, match, options, aggregator);
            RunComparisonStep(EvaluateReturnTypeChanges, match, options, aggregator);
        }

        private void EvaluateAccessModifierChanges(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch = new ItemMatch<IAccessModifiersElement<AccessModifiers>>(match.OldItem, match.NewItem);

            var results = _accessModifiersComparer.CompareItems(convertedMatch, options);

            aggregator.AddResults(results);
        }

        private static void EvaluateReturnTypeChanges(ItemMatch<T> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            if (match.OldItem.ReturnType != match.NewItem.ReturnType)
            {
                var genericTypeMatch = MapGenericTypeName(match);

                if (genericTypeMatch != match.NewItem.ReturnType)
                {
                    var args = new FormatArguments(
                        "{DefinitionType} {Identifier} return type has changed from {OldValue} to {NewValue}",
                        match.NewItem.FullName, match.OldItem.ReturnType, match.NewItem.ReturnType);

                    aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
                }
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

            var oldGenericTypes = oldDeclaringType.GenericTypeParameters.FastToList();

            if (oldGenericTypes.Count == 0)
            {
                return originalTypeName;
            }

            var newGenericTypes = newDeclaringType.GenericTypeParameters.FastToList();
            var typeIndex = oldGenericTypes.IndexOf(originalTypeName);

            if (typeIndex == -1)
            {
                return originalTypeName;
            }

            return newGenericTypes[typeIndex];
        }
    }
}