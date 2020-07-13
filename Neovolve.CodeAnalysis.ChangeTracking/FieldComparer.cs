namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class FieldComparer : MemberComparer<IFieldDefinition>, IFieldComparer
    {
        public FieldComparer(IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
        }

        protected override void EvaluateMatch(
            ItemMatch<IFieldDefinition> match,
            ComparerOptions options, ChangeResultAggregator aggregator)
        {
            RunComparisonStep(EvaluateModifierChanges, match, options, aggregator);

            base.EvaluateMatch(match, options, aggregator);
        }

        private static void EvaluateModifierChanges(
            ItemMatch<IFieldDefinition> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            var change = MemberModifiersChangeTable.CalculateChange(match);

            if (change == SemVerChangeType.None)
            {
                return;
            }

            if (match.OldItem.Modifiers == MemberModifiers.None)
            {
                // Modifiers have been added where there were previously none defined
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has added the modifiers {match.NewItem.Modifiers}");

                aggregator.AddResult(result);
            }
            else if (match.NewItem.Modifiers == MemberModifiers.None)
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