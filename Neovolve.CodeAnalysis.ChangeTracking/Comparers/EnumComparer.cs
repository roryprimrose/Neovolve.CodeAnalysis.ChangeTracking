namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class EnumComparer : ElementComparer<IEnumDefinition>, IEnumComparer
    {
        private readonly IEnumAccessModifiersComparer _enumAccessModifiersComparer;
        private readonly IEnumMemberMatchProcessor _enumMemberMatchProcessor;
        private readonly IEnumUnderlyingTypeChangeTable _underlyingTypeChangeTable;

        public EnumComparer(IEnumMemberMatchProcessor enumMemberMatchProcessor,
            IEnumAccessModifiersComparer enumAccessModifiersComparer,
            IEnumUnderlyingTypeChangeTable underlyingTypeChangeTable,
            IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
            _enumAccessModifiersComparer = enumAccessModifiersComparer
                                           ?? throw new ArgumentNullException(nameof(enumAccessModifiersComparer));
            _underlyingTypeChangeTable = underlyingTypeChangeTable
                                         ?? throw new ArgumentNullException(nameof(underlyingTypeChangeTable));
            _enumMemberMatchProcessor = enumMemberMatchProcessor
                                        ?? throw new ArgumentNullException(nameof(enumMemberMatchProcessor));
        }

        protected override void EvaluateChildElementChanges(ItemMatch<IEnumDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            base.EvaluateChildElementChanges(match, options, aggregator);

            var results =
                _enumMemberMatchProcessor.CalculateChanges(match.OldItem.Members, match.NewItem.Members, options);

            aggregator.AddResults(results);
        }

        protected override void EvaluateModifierChanges(ItemMatch<IEnumDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            base.EvaluateModifierChanges(match, options, aggregator);

            RunComparisonStep(EvaluateAccessModifierChanges, match, options, aggregator, true);
        }

        protected override void EvaluateTypeDefinitionChanges(ItemMatch<IEnumDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            base.EvaluateTypeDefinitionChanges(match, options, aggregator);

            RunComparisonStep(CompareNamespace, match, options, aggregator, true);
            RunComparisonStep(CompareUnderlyingType, match, options, aggregator, true);
        }

        private static void CompareNamespace(
            ItemMatch<IEnumDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            // Check for a change in type
            if (match.OldItem.Namespace != match.NewItem.Namespace)
            {
                var args = new FormatArguments(
                    "has changed namespace from {OldValue} to {NewValue}", match.OldItem.Namespace, match.NewItem.Namespace);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
        }

        private void CompareUnderlyingType(ItemMatch<IEnumDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldBaseType = string.Empty;
            var newBaseType = string.Empty;

            if (match.OldItem.ImplementedTypes.Count > 0)
            {
                oldBaseType = match.OldItem.ImplementedTypes.First();
            }

            if (match.NewItem.ImplementedTypes.Count > 0)
            {
                newBaseType = match.NewItem.ImplementedTypes.First();
            }

            var change = _underlyingTypeChangeTable.CalculateChange(oldBaseType, newBaseType);

            if (change == SemVerChangeType.None)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(oldBaseType))
            {
                // Underlying type has been added
                var args = new FormatArguments(
                    "has changed the underlying type from (implicit) {OldValue} to {NewValue} ",
                    "int",
                    newBaseType);

                var message = options.MessageFormatter.FormatMatch(match, ItemFormatType.ItemChanged, args);

                var result = new ComparisonResult(
                    change,
                    match.OldItem, match.NewItem,
                    message);

                aggregator.AddResult(result);
            }
            else if (string.IsNullOrWhiteSpace(newBaseType))
            {
                // Underlying type has been removed
                var args = new FormatArguments(
                    "has changed the underlying type from {OldValue} to (implicit) {NewValue}",
                    oldBaseType,
                    "int");

                var message = options.MessageFormatter.FormatMatch(match, ItemFormatType.ItemChanged, args);

                var result = new ComparisonResult(
                    change,
                    match.OldItem, match.NewItem,
                    message);

                aggregator.AddResult(result);
            }
            else
            {
                // Underlying type has been changed
                var args = new FormatArguments(
                    "has changed the underlying type from {OldValue} to {NewValue}",
                    oldBaseType,
                    newBaseType);

                var message = options.MessageFormatter.FormatMatch(match, ItemFormatType.ItemChanged, args);

                var result = new ComparisonResult(
                    change,
                    match.OldItem, match.NewItem,
                    message);

                aggregator.AddResult(result);
            }
        }

        private void EvaluateAccessModifierChanges(
            ItemMatch<IEnumDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch =
                new ItemMatch<IAccessModifiersElement<EnumAccessModifiers>>(match.OldItem, match.NewItem);

            var results = _enumAccessModifiersComparer.CompareMatch(convertedMatch, options);

            aggregator.AddResults(results);
        }
    }
}