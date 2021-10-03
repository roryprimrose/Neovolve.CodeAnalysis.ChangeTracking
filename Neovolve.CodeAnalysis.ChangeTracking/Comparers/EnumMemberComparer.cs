namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class EnumMemberComparer : ElementComparer<IEnumMemberDefinition>, IEnumMemberComparer
    {
        public EnumMemberComparer(IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
        }

        protected override void EvaluateSignatureChanges(ItemMatch<IEnumMemberDefinition> match,
            ComparerOptions options, IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));
            aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));

            base.EvaluateSignatureChanges(match, options, aggregator);

            if (match.OldItem.Name != match.NewItem.Name)
            {
                // This enum has an implicit value assigned by the compiler but has changed position
                // The underlying value of this member will most likely have changed
                var args = new FormatArguments(
                    $"has been renamed to {MessagePart.NewValue}",
                    null,
                    match.NewItem.Name);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
            else if (string.IsNullOrWhiteSpace(match.OldItem.Value)
                     && string.IsNullOrWhiteSpace(match.NewItem.Value)
                     && match.OldItem.Index != match.NewItem.Index)
            {
                // This enum has an implicit value assigned by the compiler but has changed position
                // The underlying value of this member will most likely have changed
                var args = new FormatArguments(
                    $"has an implicit value and has changed index position from {MessagePart.OldValue} to {MessagePart.NewValue}",
                    match.OldItem.Index.ToString(),
                    match.NewItem.Index.ToString());

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }

            if (match.OldItem.Value == match.NewItem.Value)
            {
                return;
            }

            // Even if the index has changed at this point it doesn't matter because the values are different

            // The value has changed in some way
            if (string.IsNullOrEmpty(match.OldItem.Value) == false
                && string.IsNullOrEmpty(match.NewItem.Value) == false)
            {
                EvaluateMemberValueChanged(match, aggregator, options);
            }
            else if (string.IsNullOrEmpty(match.OldItem.Value))
            {
                // The new member has a value but the old member doesn't
                // This has changed from an implicit compile time assigned value to an explicit value
                // We need to assume that this is a breaking change
                var args = new FormatArguments(
                    $"has redefined the underlying value from an implicit value to the explicit value of {MessagePart.NewValue}",
                    null,
                    match.NewItem.Value);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
            else
            {
                // The old member has a value but the new member doesn't
                // This has changed from an explicit value to an implicit compile time assigned value
                // We need to assume that this is a breaking change
                var args = new FormatArguments(
                    $"has redefined the underlying value from the explicit value of {MessagePart.OldValue} to an implicit value",
                    match.OldItem.Value,
                    null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
        }

        private static void EvaluateMemberValueChanged(ItemMatch<IEnumMemberDefinition> match,
            IChangeResultAggregator aggregator,
            ComparerOptions options)
        {
            // We want to check if the change of value is whitespace or ordering of bitwise values
            var oldValue = match.OldItem.Value;
            var newValue = match.NewItem.Value;

            var oldParts = oldValue.Split('|').Select(x => x.Trim()).OrderBy(x => x);
            var newParts = newValue.Split('|').Select(x => x.Trim()).OrderBy(x => x);

            if (oldParts.SequenceEqual(newParts))
            {
                // The change in the value is either whitespace and/or ordering of the bitwise values
                // This is not actually a change of the compiled value
                return;
            }

            // Both members have values so the value of the enum member has changed
            var args = new FormatArguments(
                $"has changed the underlying value from {MessagePart.OldValue} to {MessagePart.NewValue}",
                match.OldItem.Value,
                match.NewItem.Value);

            aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
        }
    }
}