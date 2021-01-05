namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class ClassComparer : TypeComparer<IClassDefinition>, IClassComparer
    {
        private readonly IClassModifiersComparer _classModifiersComparer;

        public ClassComparer(IAccessModifiersComparer accessModifiersComparer,
            IClassModifiersComparer classModifiersComparer,
            IFieldMatchProcessor fieldProcessor,
            IPropertyMatchProcessor propertyProcessor,
            IAttributeMatchProcessor attributeProcessor) : base(
            accessModifiersComparer,
            fieldProcessor,
            propertyProcessor,
            attributeProcessor)
        {
            _classModifiersComparer =
                classModifiersComparer ?? throw new ArgumentNullException(nameof(classModifiersComparer));
        }

        protected override void EvaluateMatch(ItemMatch<IClassDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            base.EvaluateMatch(match, options, aggregator);

            RunComparisonStep(EvaluateClassModifierChanges, match, options, aggregator);
        }

        private void EvaluateClassModifierChanges(
            ItemMatch<IClassDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch =
                new ItemMatch<IModifiersElement<ClassModifiers>>(match.OldItem, match.NewItem);

            var results = _classModifiersComparer.CompareItems(convertedMatch, options);

            aggregator.AddResults(results);
        }
    }
}