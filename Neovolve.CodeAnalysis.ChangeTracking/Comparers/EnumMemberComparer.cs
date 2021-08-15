namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
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
                    "{DefinitionType} {Identifier} has been renamed to {NewValue}",
                    match.NewItem.FullName,
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
                    "{DefinitionType} {Identifier} has an implicit value and has changed index position from {OldValue} to {NewValue}",
                    match.NewItem.FullName,
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
                // Both members have values so the value of the enum member has changed
                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has changed the underlying value from {OldValue} to {NewValue}",
                    match.NewItem.FullName,
                    match.OldItem.Value,
                    match.NewItem.Value);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
            else if (string.IsNullOrEmpty(match.OldItem.Value))
            {
                // The new member has a value but the old member doesn't
                // This has changed from an implicit compile time assigned value to an explicit value
                // We need to assume that this is a breaking change
                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has redefined the underlying value from an implicit value to the explicit value of {NewValue}",
                    match.NewItem.FullName,
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
                    "{DefinitionType} {Identifier} has redefined the underlying value from the explicit value of {OldValue} to an implicit value",
                    match.NewItem.FullName,
                    match.OldItem.Value,
                    null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
        }
    }
}