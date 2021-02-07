namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class StructComparer : TypeComparer<IStructDefinition>, IStructComparer
    {
        private readonly IStructModifiersComparer _structModifiersComparer;

        public StructComparer(IAccessModifiersComparer accessModifiersComparer,
            IStructModifiersComparer structModifiersComparer,
            IFieldMatchProcessor fieldProcessor,
            IPropertyMatchProcessor propertyProcessor,
            IMethodMatchProcessor methodProcessor,
            IAttributeMatchProcessor attributeProcessor) : base(
            accessModifiersComparer,
            fieldProcessor,
            propertyProcessor,
            methodProcessor,
            attributeProcessor)
        {
            _structModifiersComparer =
                structModifiersComparer ?? throw new ArgumentNullException(nameof(structModifiersComparer));
        }

        protected override void EvaluateMatch(ItemMatch<IStructDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            base.EvaluateMatch(match, options, aggregator);

            RunComparisonStep(EvaluateStructModifierChanges, match, options, aggregator);
        }

        private void EvaluateStructModifierChanges(
            ItemMatch<IStructDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch =
                new ItemMatch<IModifiersElement<StructModifiers>>(match.OldItem, match.NewItem);

            var results = _structModifiersComparer.CompareItems(convertedMatch, options);

            aggregator.AddResults(results);
        }
    }
}