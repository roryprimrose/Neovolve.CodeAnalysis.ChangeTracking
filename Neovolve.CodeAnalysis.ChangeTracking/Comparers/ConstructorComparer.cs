namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class ConstructorComparer : MemberComparer<IConstructorDefinition>, IConstructorComparer
    {
        private readonly IParameterComparer _parameterComparer;

        public ConstructorComparer(
            IAccessModifiersComparer accessModifiersComparer,
            IParameterComparer parameterComparer,
            IAttributeMatchProcessor attributeProcessor) : base(
            accessModifiersComparer, attributeProcessor)
        {
            _parameterComparer = parameterComparer ?? throw new ArgumentNullException(nameof(parameterComparer));
        }

        protected override void EvaluateModifierChanges(ItemMatch<IConstructorDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            base.EvaluateModifierChanges(match, options, aggregator);

            RunComparisonStep(EvaluateMethodModifierChanges, match, options, aggregator, true);
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
                    $"has added the {{NewValue}} modifier",
                    null,
                    "static");

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
            else
            {
                // The constructor was a static constructor but is now an instance
                var args = new FormatArguments(
                    $"has removed the {{OldValue}} modifier",
                    "static",
                    null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
        }

        protected override void EvaluateSignatureChanges(ItemMatch<IConstructorDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            base.EvaluateSignatureChanges(match, options, aggregator);

            RunComparisonStep(EvaluateParameterChanges, match, options, aggregator);
        }

        private void EvaluateParameterChanges(
            ItemMatch<IConstructorDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldParameters = match.OldItem.Parameters.FastToList();
            var newParameters = match.NewItem.Parameters.FastToList();

            if (oldParameters.Count == 0
                && newParameters.Count == 0)
            {
                return;
            }

            var parameterShift = oldParameters.Count - newParameters.Count;

            if (parameterShift != 0)
            {
                var changeLabel = parameterShift > 0 ? "removed" : "added";
                var shiftAmount = Math.Abs(parameterShift);

                // One or more generic type parameters have been added or removed
                var suffix = shiftAmount == 1 ? "" : "s";
                var args = new FormatArguments(
                    $"has {changeLabel} {shiftAmount} parameter{suffix}",
                    null,
                    null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);

                // No need to look into how the generic type has changed
                return;
            }

            // We have the same number of parameters, compare them
            for (var index = 0; index < oldParameters.Count; index++)
            {
                var oldParameter = oldParameters[index];
                var newParameter = newParameters[index];
                var parameterMatch = new ItemMatch<IParameterDefinition>(oldParameter, newParameter);

                var parameterChanges = _parameterComparer.CompareMatch(parameterMatch, options);

                aggregator.AddResults(parameterChanges);
            }
        }
    }
}