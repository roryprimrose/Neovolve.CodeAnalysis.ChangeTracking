namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public abstract class BaseTypeComparer<T> : ElementComparer<T>, IBaseTypeComparer<T> where T : IBaseTypeDefinition
    {
        private readonly IAccessModifiersComparer _accessModifiersComparer;

        protected BaseTypeComparer(IAccessModifiersComparer accessModifiersComparer,
            IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
            _accessModifiersComparer = accessModifiersComparer
                                       ?? throw new ArgumentNullException(nameof(accessModifiersComparer));
        }

        protected override void EvaluateModifierChanges(ItemMatch<T> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            base.EvaluateModifierChanges(match, options, aggregator);

            RunComparisonStep(EvaluateAccessModifierChanges, match, options, aggregator, true);
        }

        private void EvaluateAccessModifierChanges(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch = new ItemMatch<IAccessModifiersElement<AccessModifiers>>(match.OldItem, match.NewItem);

            var results = _accessModifiersComparer.CompareMatch(convertedMatch, options);

            aggregator.AddResults(results);
        }
    }
}