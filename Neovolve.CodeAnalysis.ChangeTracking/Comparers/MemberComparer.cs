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
            RunComparisonStep(EvaluateAccessModifierChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateReturnTypeChanges, match, options, aggregator, true);
        }

        private void EvaluateAccessModifierChanges(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch = new ItemMatch<IAccessModifiersElement<AccessModifiers>>(match.OldItem, match.NewItem);

            var results = _accessModifiersComparer.CompareMatch(convertedMatch, options);

            aggregator.AddResults(results);
        }

        private static void EvaluateReturnTypeChanges(ItemMatch<T> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldType = match.OldItem.ReturnType;
            var newType = match.NewItem.ReturnType;

            var oldMappedType =
                match.OldItem.DeclaringType.GetMatchingGenericType(oldType, match.NewItem.DeclaringType);
            
            if (oldMappedType != newType)
            {
                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} return type has changed from {OldValue} to {NewValue}",
                    match.NewItem.FullName, match.OldItem.ReturnType, match.NewItem.ReturnType);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
        }
    }
}