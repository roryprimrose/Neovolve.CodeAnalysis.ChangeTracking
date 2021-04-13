namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class MethodComparer : MemberComparer<IMethodDefinition>, IMethodComparer
    {
        private readonly IGenericTypeElementComparer _genericTypeElementComparer;
        private readonly IMethodModifiersComparer _methodModifiersComparer;
        private readonly IParameterComparer _parameterComparer;

        public MethodComparer(IAccessModifiersComparer accessModifiersComparer,
            IMethodModifiersComparer methodModifiersComparer,
            IGenericTypeElementComparer genericTypeElementComparer,
            IParameterComparer parameterComparer,
            IAttributeMatchProcessor attributeProcessor) : base(
            accessModifiersComparer, attributeProcessor)
        {
            _methodModifiersComparer = methodModifiersComparer
                                       ?? throw new ArgumentNullException(nameof(methodModifiersComparer));
            _genericTypeElementComparer = genericTypeElementComparer
                                          ?? throw new ArgumentNullException(nameof(genericTypeElementComparer));
            _parameterComparer = parameterComparer ?? throw new ArgumentNullException(nameof(parameterComparer));
        }

        protected override void EvaluateMatch(
            ItemMatch<IMethodDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            RunComparisonStep(EvaluateMethodModifierChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateMethodNameChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateGenericTypeDefinitionChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateParameterChanges, match, options, aggregator);

            base.EvaluateMatch(match, options, aggregator);
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
                "{DefinitionType} {Identifier} has been renamed to {NewValue}",
                match.OldItem.FullName,
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
                    $"{{DefinitionType}} {{Identifier}} has {changeLabel} {shiftAmount} parameter{suffix}",
                    match.NewItem.FullName,
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