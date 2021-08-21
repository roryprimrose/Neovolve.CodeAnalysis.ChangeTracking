namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class EnumComparer : BaseTypeComparer<IEnumDefinition>, IEnumComparer
    {
        private readonly IEnumMemberMatchProcessor _enumMemberMatchProcessor;

        public EnumComparer(IEnumMemberMatchProcessor enumMemberMatchProcessor,
            IAccessModifiersComparer accessModifiersComparer,
            IAttributeMatchProcessor attributeProcessor) : base(accessModifiersComparer, attributeProcessor)
        {
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
    }
}