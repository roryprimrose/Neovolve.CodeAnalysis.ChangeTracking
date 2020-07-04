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

            if (match.OldItem.AccessModifier == AccessModifier.None)
            {
                // Modifiers have been added where there were previously none defined
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has added the modifiers {match.NewItem.AccessModifier}");

                aggregator.AddResult(result);
            }
            else if (match.NewItem.AccessModifier == AccessModifier.None)
            {
                // All previous modifiers have been removed
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has removed the modifiers {match.OldItem.AccessModifier}");

                aggregator.AddResult(result);
            }
            else
            {
                // Modifiers have been changed
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has changed modifiers from {match.OldItem.AccessModifier} to {match.NewItem.AccessModifier}");

                aggregator.AddResult(result);
            }
        }
    }
}