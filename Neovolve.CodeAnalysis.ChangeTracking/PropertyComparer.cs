namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyComparer : MemberComparer<IPropertyDefinition>, IPropertyComparer
    {
        public PropertyComparer(IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
        }

        protected override void EvaluateMatch(
            ItemMatch<IPropertyDefinition> match,
            ComparerOptions options, ChangeResultAggregator aggregator)
        {
            RunComparisonStep(EvaluateModifierChanges, match, options, aggregator);
            RunComparisonStep(EvaluatePropertyAccessors, match, options, aggregator);

            base.EvaluateMatch(match, options, aggregator);
        }

        private static void EvaluatePropertyAccessors(
            ItemMatch<IPropertyDefinition> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            // Calculate breaking changes
            if (match.OldItem.CanRead
                && match.NewItem.CanRead == false)
            {
                var message = match.NewItem.Description + " removed the get accessor";

                var result = ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);

                aggregator.AddResult(result);
            }
            else if (match.OldItem.CanRead == false
                     && match.NewItem.CanRead)
            {
                var message = match.NewItem.Description + " added a get accessor";

                var result = ComparisonResult.ItemChanged(SemVerChangeType.Feature, match, message);

                aggregator.AddResult(result);
            }

            if (match.OldItem.CanWrite
                && match.NewItem.CanWrite == false)
            {
                var message = match.NewItem.Description + " removed the set accessor";

                var result = ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);

                aggregator.AddResult(result);
            }
            else if (match.OldItem.CanWrite == false
                     && match.NewItem.CanWrite)
            {
                var message = match.NewItem.Description + " added a set accessor";

                var result = ComparisonResult.ItemChanged(SemVerChangeType.Feature, match, message);

                aggregator.AddResult(result);
            }
        }

        private static void EvaluateModifierChanges(
            ItemMatch<IPropertyDefinition> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            var change = PropertyModifierChangeTable.CalculateChange(match);

            if (change == SemVerChangeType.None)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(match.OldItem.Modifiers))
            {
                // Modifiers have been added where there were previously none defined
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has added the modifiers {match.NewItem.Modifiers}");

                aggregator.AddResult(result);
            }
            else if (string.IsNullOrWhiteSpace(match.NewItem.Modifiers))
            {
                // All previous modifiers have been removed
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has removed the modifiers {match.OldItem.Modifiers}");

                aggregator.AddResult(result);
            }
            else
            {
                // Modifiers have been changed
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has changed modifiers from {match.OldItem.Modifiers} to {match.NewItem.Modifiers}");

                aggregator.AddResult(result);
            }
        }
    }
}