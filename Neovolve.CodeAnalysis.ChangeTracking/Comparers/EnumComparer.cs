namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class EnumComparer : ElementComparer<IEnumDefinition>, IEnumComparer
    {
        private readonly IEnumAccessModifiersComparer _enumAccessModifiersComparer;
        private readonly IEnumMemberMatchProcessor _enumMemberMatchProcessor;

        public EnumComparer(IEnumMemberMatchProcessor enumMemberMatchProcessor,
            IEnumAccessModifiersComparer enumAccessModifiersComparer,
            IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
            _enumAccessModifiersComparer = enumAccessModifiersComparer
                                       ?? throw new ArgumentNullException(nameof(enumAccessModifiersComparer));
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