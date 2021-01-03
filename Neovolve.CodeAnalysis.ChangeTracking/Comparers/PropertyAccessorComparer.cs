namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class PropertyAccessorComparer : ElementComparer<IPropertyAccessorDefinition>, IPropertyAccessorComparer
    {
        private readonly IPropertyAccessorAccessModifiersComparer _propertyAccessorAccessModifiersComparer;

        public PropertyAccessorComparer(
            IPropertyAccessorAccessModifiersComparer propertyAccessorAccessModifiersComparer,
            IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
            _propertyAccessorAccessModifiersComparer = propertyAccessorAccessModifiersComparer
                                                       ?? throw new ArgumentNullException(
                                                           nameof(propertyAccessorAccessModifiersComparer));
        }

        protected override void EvaluateMatch(ItemMatch<IPropertyAccessorDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            RunComparisonStep(EvaluateAccessModifierChanges, match, options, aggregator, true);
        }

        private void EvaluateAccessModifierChanges(
            ItemMatch<IPropertyAccessorDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch =
                new ItemMatch<IAccessModifiersElement<PropertyAccessorAccessModifiers>>(match.OldItem, match.NewItem);

            var results = _propertyAccessorAccessModifiersComparer.CompareItems(convertedMatch, options);

            aggregator.AddResults(results);
        }
    }
}