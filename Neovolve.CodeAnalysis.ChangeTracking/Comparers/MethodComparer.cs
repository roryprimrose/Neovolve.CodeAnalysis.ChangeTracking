namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class MethodComparer : MemberComparer<IMethodDefinition>, IMethodComparer
    {
        private readonly IGenericTypeElementComparer _genericTypeElementComparer;
        private readonly IMethodModifiersComparer _methodModifiersComparer;
        private readonly IParameterMatchProcessor _parameterProcessor;

        public MethodComparer(IAccessModifiersComparer accessModifiersComparer,
            IMethodModifiersComparer methodModifiersComparer,
            IGenericTypeElementComparer genericTypeElementComparer,
            IParameterMatchProcessor parameterProcessor,
            IAttributeMatchProcessor attributeProcessor) : base(
            accessModifiersComparer, attributeProcessor)
        {
            _methodModifiersComparer = methodModifiersComparer
                                       ?? throw new ArgumentNullException(nameof(methodModifiersComparer));
            _genericTypeElementComparer = genericTypeElementComparer
                                          ?? throw new ArgumentNullException(nameof(genericTypeElementComparer));
            _parameterProcessor = parameterProcessor ?? throw new ArgumentNullException(nameof(parameterProcessor));
        }

        protected override void EvaluateModifierChanges(ItemMatch<IMethodDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            base.EvaluateModifierChanges(match, options, aggregator);

            RunComparisonStep(EvaluateMethodModifierChanges, match, options, aggregator, true);
        }

        protected override void EvaluateSignatureChanges(ItemMatch<IMethodDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            base.EvaluateSignatureChanges(match, options, aggregator);

            RunComparisonStep(EvaluateMethodNameChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateGenericTypeDefinitionChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateParameterChanges, match, options, aggregator);
        }

        private static void EvaluateMethodNameChanges(
            ItemMatch<IMethodDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            if (match.OldItem.RawName == match.NewItem.RawName)
            {
                return;
            }

            var args = new FormatArguments(
                $"has been renamed to {MessagePart.NewValue}",
                null,
                match.NewItem.Name);

            aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
        }

        private void EvaluateGenericTypeDefinitionChanges(
            ItemMatch<IMethodDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch = new ItemMatch<IGenericTypeElement>(match.OldItem, match.NewItem);

            var results = _genericTypeElementComparer.CompareMatch(convertedMatch, options);

            aggregator.AddResults(results);
        }

        private void EvaluateMethodModifierChanges(
            ItemMatch<IMethodDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch = new ItemMatch<IModifiersElement<MethodModifiers>>(match.OldItem, match.NewItem);

            var results = _methodModifiersComparer.CompareMatch(convertedMatch, options);

            aggregator.AddResults(results);
        }

        private void EvaluateParameterChanges(
            ItemMatch<IMethodDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var changes =
                _parameterProcessor.CalculateChanges(match.OldItem.Parameters, match.NewItem.Parameters, options);

            aggregator.AddResults(changes);
        }
    }
}