namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyComparer : MemberComparer<IPropertyDefinition>, IPropertyComparer
    {
        private readonly IPropertyAccessorMatchProcessor _accessorProcessor;

        public PropertyComparer(IPropertyAccessorMatchProcessor accessorProcessor,
            IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
            _accessorProcessor = accessorProcessor ?? throw new ArgumentNullException(nameof(accessorProcessor));
        }

        protected override void EvaluateMatch(
            ItemMatch<IPropertyDefinition> match,
            ComparerOptions options, ChangeResultAggregator aggregator)
        {
            RunComparisonStep(EvaluateModifierChanges, match, options, aggregator);
            RunComparisonStep(EvaluatePropertyAccessors, match, options, aggregator);

            base.EvaluateMatch(match, options, aggregator);
        }

        private static void EvaluateModifierChanges(
            ItemMatch<IPropertyDefinition> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            var change = MemberModifiersChangeTable.CalculateChange(match);

            if (change == SemVerChangeType.None)
            {
                return;
            }

            var newModifiers = match.NewItem.GetDeclaredModifiers();
            var oldModifiers = match.OldItem.GetDeclaredModifiers();

            if (string.IsNullOrWhiteSpace(oldModifiers))
            {
                // Modifiers have been added where there were previously none defined
                var suffix = string.Empty;

                if (newModifiers.Contains(" "))
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has added the {newModifiers} modifier{suffix}");

                aggregator.AddResult(result);
            }
            else if (string.IsNullOrWhiteSpace(newModifiers))
            {
                // All previous modifiers have been removed
                var suffix = string.Empty;

                if (oldModifiers.Contains(" "))
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has removed the {oldModifiers} modifier{suffix}");

                aggregator.AddResult(result);
            }
            else
            {
                // Modifiers have been changed
                var suffix = string.Empty;

                if (oldModifiers.Contains(" "))
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has changed the modifier{suffix} from {oldModifiers} to {newModifiers}");

                aggregator.AddResult(result);
            }
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

        private void EvaluatePropertyAccessors(
            ItemMatch<IPropertyDefinition> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            var oldAccessors = GetAccessorList(match.OldItem);
            var newAccessors = GetAccessorList(match.NewItem);

            var changes = _accessorProcessor.CalculateChanges(oldAccessors, newAccessors, options);

            aggregator.AddResults(changes);
        }
    }
}