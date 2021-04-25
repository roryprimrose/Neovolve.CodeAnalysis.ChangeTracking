namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class PropertyComparer : MemberComparer<IPropertyDefinition>, IPropertyComparer
    {
        private readonly IPropertyAccessorMatchProcessor _accessorProcessor;
        private readonly IPropertyModifiersComparer _propertyModifiersComparer;

        public PropertyComparer(
            IAccessModifiersComparer accessModifiersComparer, IPropertyModifiersComparer propertyModifiersComparer,
            IPropertyAccessorMatchProcessor accessorProcessor,
            IAttributeMatchProcessor attributeProcessor) : base(accessModifiersComparer, attributeProcessor)
        {
            _propertyModifiersComparer = propertyModifiersComparer
                                         ?? throw new ArgumentNullException(nameof(propertyModifiersComparer));
            _accessorProcessor = accessorProcessor ?? throw new ArgumentNullException(nameof(accessorProcessor));
        }

        protected override void EvaluateModifierChanges(ItemMatch<IPropertyDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            base.EvaluateModifierChanges(match, options, aggregator);

            RunComparisonStep(EvaluatePropertyModifierChanges, match, options, aggregator);
        }

        protected override void EvaluateSignatureChanges(ItemMatch<IPropertyDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            base.EvaluateSignatureChanges(match, options, aggregator);

            RunComparisonStep(EvaluatePropertyAccessors, match, options, aggregator);
        }

        private static IEnumerable<IPropertyAccessorDefinition> GetAccessorList(IPropertyDefinition definition)
        {
            if (definition.GetAccessor != null)
            {
                yield return definition.GetAccessor;
            }

            if (definition.SetAccessor != null)
            {
                yield return definition.SetAccessor;
            }
        }

        private void EvaluatePropertyAccessors(
            ItemMatch<IPropertyDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldAccessors = GetAccessorList(match.OldItem);
            var newAccessors = GetAccessorList(match.NewItem);

            var changes = _accessorProcessor.CalculateChanges(oldAccessors, newAccessors, options);

            aggregator.AddResults(changes);
        }

        private void EvaluatePropertyModifierChanges(
            ItemMatch<IPropertyDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch =
                new ItemMatch<IModifiersElement<PropertyModifiers>>(match.OldItem, match.NewItem);

            var results = _propertyModifiersComparer.CompareMatch(convertedMatch, options);

            aggregator.AddResults(results);
        }
    }
}