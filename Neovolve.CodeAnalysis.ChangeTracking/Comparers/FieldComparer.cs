namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class FieldComparer : MemberComparer<IFieldDefinition>, IFieldComparer
    {
        private readonly IFieldModifiersComparer _fieldModifiersComparer;

        public FieldComparer(
            IAccessModifiersComparer accessModifiersComparer, IFieldModifiersComparer fieldModifiersComparer,
            IAttributeMatchProcessor attributeProcessor) : base(
            accessModifiersComparer, attributeProcessor)
        {
            _fieldModifiersComparer =
                fieldModifiersComparer ?? throw new ArgumentNullException(nameof(fieldModifiersComparer));
        }

        protected override void EvaluateModifierChanges(ItemMatch<IFieldDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            base.EvaluateModifierChanges(match, options, aggregator);

            RunComparisonStep(EvaluateFieldModifierChanges, match, options, aggregator);
        }

        private void EvaluateFieldModifierChanges(
            ItemMatch<IFieldDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch =
                new ItemMatch<IModifiersElement<FieldModifiers>>(match.OldItem, match.NewItem);

            var results = _fieldModifiersComparer.CompareMatch(convertedMatch, options);

            aggregator.AddResults(results);
        }
    }
}