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

            var newModifiers = match.NewItem.GetDeclaredAccessModifiers();
            var oldModifiers = match.OldItem.GetDeclaredAccessModifiers();

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
                    $"{match.NewItem.Description} has added the {newModifiers} access modifier{suffix}");

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
                    $"{match.NewItem.Description} has removed the {oldModifiers} access modifier{suffix}");

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
                    $"{match.NewItem.Description} has changed the access modifier{suffix} from {oldModifiers} to {newModifiers}");

                aggregator.AddResult(result);
            }
        }

    }
}