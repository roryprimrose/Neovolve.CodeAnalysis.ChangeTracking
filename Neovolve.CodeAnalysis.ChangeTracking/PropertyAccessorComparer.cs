namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyAccessorComparer : ElementComparer<IPropertyAccessorDefinition>, IPropertyAccessorComparer
    {
        public PropertyAccessorComparer(IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
        }

        protected override void EvaluateMatch(ItemMatch<IPropertyAccessorDefinition> match, ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            RunComparisonStep(EvaluateAccessModifierChanges, match, options, aggregator, true);
        }

        private static void EvaluateAccessModifierChanges(
            ItemMatch<IPropertyAccessorDefinition> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            var change = PropertyAccessorAccessModifierChangeTable.CalculateChange(match);

            if (change == SemVerChangeType.None)
            {
                return;
            }

            if (match.OldItem.AccessModifier == PropertyAccessorAccessModifier.None)
            {
                // Modifiers have been added where there were previously none defined
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has added the access modifiers {match.NewItem.AccessModifier}");

                aggregator.AddResult(result);
            }
            else if (match.NewItem.AccessModifier == PropertyAccessorAccessModifier.None)
            {
                // All previous modifiers have been removed
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has removed the access modifiers {match.OldItem.AccessModifier}");

                aggregator.AddResult(result);
            }
            else
            {
                // Modifiers have been changed
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has changed access modifiers from {match.OldItem.AccessModifier} to {match.NewItem.AccessModifier}");

                aggregator.AddResult(result);
            }
        }

    }
}