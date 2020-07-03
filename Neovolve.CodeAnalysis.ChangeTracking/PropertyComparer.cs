namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyComparer : MemberComparer<IPropertyDefinition>, IPropertyComparer
    {
        public PropertyComparer(IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
        }

        protected override IEnumerable<ComparisonResult> EvaluateMatch(ItemMatch<IPropertyDefinition> match,
            ComparerOptions options)
        {
            // Include results from the base class
            foreach (var result in base.EvaluateMatch(match, options))
            {
                yield return result;
            }

            foreach (var comparisonResult in EvaluateModifierChanges(match))
            {
                yield return comparisonResult;
            }

            // Calculate breaking changes
            if (match.OldItem.CanRead
                && match.NewItem.CanRead == false)
            {
                var message = match.NewItem.Description + " removed the get accessor";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);
            }
            else if (match.OldItem.CanRead == false
                     && match.NewItem.CanRead)
            {
                var message = match.NewItem.Description + " added a get accessor";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match, message);
            }

            if (match.OldItem.CanWrite
                && match.NewItem.CanWrite == false)
            {
                var message = match.NewItem.Description + " removed the set accessor";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);
            }
            else if (match.OldItem.CanWrite == false
                     && match.NewItem.CanWrite)
            {
                var message = match.NewItem.Description + " added a set accessor";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match, message);
            }
        }

        private static IEnumerable<ComparisonResult> EvaluateModifierChanges(ItemMatch<IPropertyDefinition> match)
        {
            var change = PropertyAccessorChangeTable.CalculateChange(match);

            if (change == SemVerChangeType.None)
            {
                yield break;
            }

            if (string.IsNullOrWhiteSpace(match.OldItem.Modifiers))
            {
                // Modifiers have been added where there were previously none defined
                yield return ComparisonResult.ItemChanged(change, match,
                    $"{match.NewItem.Description} has added the modifiers {match.NewItem.Modifiers}");
            }
            else if (string.IsNullOrWhiteSpace(match.NewItem.Modifiers))
            {
                // All previous modifiers have been removed
                yield return ComparisonResult.ItemChanged(change, match,
                    $"{match.NewItem.Description} has removed the modifiers {match.OldItem.Modifiers}");
            }
            else
            {
                // Modifiers have been changed
                yield return ComparisonResult.ItemChanged(change, match,
                    $"{match.NewItem.Description} has changed modifiers from {match.OldItem.Modifiers} to {match.NewItem.Modifiers}");
            }
        }
    }
}