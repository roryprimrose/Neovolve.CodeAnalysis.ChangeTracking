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

        protected override void EvaluateAccessModifierChanges(
            ItemMatch<IPropertyAccessorDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch =
                new ItemMatch<IAccessModifiersElement<PropertyAccessorAccessModifiers>>(match.OldItem, match.NewItem);

            var results = _propertyAccessorAccessModifiersComparer.CompareMatch(convertedMatch, options);

            aggregator.AddResults(results);
        }
    }
}