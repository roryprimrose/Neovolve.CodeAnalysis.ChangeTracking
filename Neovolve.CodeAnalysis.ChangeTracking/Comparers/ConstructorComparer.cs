namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class ConstructorComparer : MemberComparer<IConstructorDefinition>, IConstructorComparer
    {
        private readonly IParameterMatchProcessor _parameterProcessor;

        public ConstructorComparer(
            IAccessModifiersComparer accessModifiersComparer,
            IParameterMatchProcessor parameterProcessor,
            IAttributeMatchProcessor attributeProcessor) : base(
            accessModifiersComparer, attributeProcessor)
        {
            _parameterProcessor = parameterProcessor ?? throw new ArgumentNullException(nameof(parameterProcessor));
        }

        protected override void EvaluateModifierChanges(ItemMatch<IConstructorDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            base.EvaluateModifierChanges(match, options, aggregator);

            RunComparisonStep(EvaluateMethodModifierChanges, match, options, aggregator, true);
        }

        protected override void EvaluateSignatureChanges(ItemMatch<IConstructorDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            base.EvaluateSignatureChanges(match, options, aggregator);

            RunComparisonStep(EvaluateParameterChanges, match, options, aggregator);
        }

        private static void EvaluateMethodModifierChanges(
            ItemMatch<IConstructorDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            if (match.OldItem.Modifiers == match.NewItem.Modifiers)
            {
                // There is no change between static and instance constructors
                return;
            }

            if (match.OldItem.Modifiers == ConstructorModifiers.None)
            {
                // The constructor was an instance constructor but is now a static
                var args = new FormatArguments(
                    $"has added the {MessagePart.NewValue} modifier",
                    null,
                    "static");

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
            else
            {
                // The constructor was a static constructor but is now an instance
                var args = new FormatArguments(
                    $"has removed the {MessagePart.OldValue} modifier",
                    "static",
                    null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
        }

        private void EvaluateParameterChanges(
            ItemMatch<IConstructorDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var changes =
                _parameterProcessor.CalculateChanges(match.OldItem.Parameters, match.NewItem.Parameters, options);

            aggregator.AddResults(changes);
        }
    }
}