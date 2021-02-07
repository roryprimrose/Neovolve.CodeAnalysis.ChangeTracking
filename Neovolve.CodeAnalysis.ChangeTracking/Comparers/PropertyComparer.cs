namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class PropertyComparer : MemberComparer<IPropertyDefinition>, IPropertyComparer
    {
        private readonly IPropertyAccessorMatchProcessor _accessorProcessor;
        private readonly IMemberModifiersComparer _memberModifiersComparer;

        public PropertyComparer(
            IAccessModifiersComparer accessModifiersComparer, IMemberModifiersComparer memberModifiersComparer,
            IPropertyAccessorMatchProcessor accessorProcessor,
            IAttributeMatchProcessor attributeProcessor) : base(accessModifiersComparer, attributeProcessor)
        {
            _memberModifiersComparer = memberModifiersComparer
                                       ?? throw new ArgumentNullException(nameof(memberModifiersComparer));
            _accessorProcessor = accessorProcessor ?? throw new ArgumentNullException(nameof(accessorProcessor));
        }

        protected override void EvaluateMatch(
            ItemMatch<IPropertyDefinition> match,
            ComparerOptions options, IChangeResultAggregator aggregator)
        {
            RunComparisonStep(EvaluateModifierChanges, match, options, aggregator);
            RunComparisonStep(EvaluatePropertyAccessors, match, options, aggregator);

            base.EvaluateMatch(match, options, aggregator);
        }

        private static IEnumerable<IPropertyAccessorDefinition> GetAccessorList(IPropertyDefinition definition)
        {
            var accessors = new List<IPropertyAccessorDefinition>();

            if (definition.GetAccessor != null)
            {
                accessors.Add(definition.GetAccessor);
            }

            if (definition.SetAccessor != null)
            {
                accessors.Add(definition.SetAccessor);
            }

            return accessors;
        }

        private void EvaluateModifierChanges(
            ItemMatch<IPropertyDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch =
                new ItemMatch<IModifiersElement<MemberModifiers>>(match.OldItem, match.NewItem);

            var results = _memberModifiersComparer.CompareItems(convertedMatch, options);

            aggregator.AddResults(results);
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
    }
}