namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class StructComparer : TypeComparer<IStructDefinition>, IStructComparer
    {
        private readonly IConstructorMatchProcessor _constructorProcessor;
        private readonly IFieldMatchProcessor _fieldProcessor;
        private readonly IStructModifiersComparer _structModifiersComparer;

        public StructComparer(IAccessModifiersComparer accessModifiersComparer,
            IStructModifiersComparer structModifiersComparer,
            IGenericTypeElementComparer genericTypeElementComparer,
            IFieldMatchProcessor fieldProcessor,
            IConstructorMatchProcessor constructorProcessor,
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
            _constructorProcessor =
                constructorProcessor ?? throw new ArgumentNullException(nameof(constructorProcessor));
        }

        protected override void EvaluateChildElementChanges(ItemMatch<IStructDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            RunComparisonStep(EvaluateFieldChanges, match, options, aggregator);
            RunComparisonStep(EvaluateConstructorChanges, match, options, aggregator);

            base.EvaluateChildElementChanges(match, options, aggregator);
        }

        protected override void EvaluateModifierChanges(ItemMatch<IStructDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            base.EvaluateModifierChanges(match, options, aggregator);

            RunComparisonStep(EvaluateStructModifierChanges, match, options, aggregator);
        }

        private void EvaluateConstructorChanges(
            ItemMatch<IStructDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var changes =
                _constructorProcessor.CalculateChanges(match.OldItem.Constructors, match.NewItem.Constructors, options);

            aggregator.AddResults(changes);
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