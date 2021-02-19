namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class StructComparer : TypeComparer<IStructDefinition>, IStructComparer
    {
        private readonly IFieldMatchProcessor _fieldProcessor;
        private readonly IStructModifiersComparer _structModifiersComparer;

        public StructComparer(IAccessModifiersComparer accessModifiersComparer,
            IStructModifiersComparer structModifiersComparer,
            IGenericTypeElementComparer genericTypeElementComparer,
            IFieldMatchProcessor fieldProcessor,
            IPropertyMatchProcessor propertyProcessor,
            IMethodMatchProcessor methodProcessor,
            IAttributeMatchProcessor attributeProcessor) : base(
            accessModifiersComparer,
            genericTypeElementComparer,
            propertyProcessor,
            methodProcessor,
            attributeProcessor)
        {
            _structModifiersComparer =
                structModifiersComparer ?? throw new ArgumentNullException(nameof(structModifiersComparer));
            _fieldProcessor = fieldProcessor ?? throw new ArgumentNullException(nameof(fieldProcessor));
        }

        protected override void EvaluateMatch(ItemMatch<IStructDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            base.EvaluateMatch(match, options, aggregator);

            RunComparisonStep(EvaluateStructModifierChanges, match, options, aggregator);
            RunComparisonStep(EvaluateFieldChanges, match, options, aggregator);
        }

        private void EvaluateFieldChanges(
            ItemMatch<IStructDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var changes = _fieldProcessor.CalculateChanges(match.OldItem.Fields, match.NewItem.Fields, options);

            aggregator.AddResults(changes);
        }

        private void EvaluateStructModifierChanges(
            ItemMatch<IStructDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch =
                new ItemMatch<IModifiersElement<StructModifiers>>(match.OldItem, match.NewItem);

            var results = _structModifiersComparer.CompareMatch(convertedMatch, options);

            aggregator.AddResults(results);
        }
    }
}