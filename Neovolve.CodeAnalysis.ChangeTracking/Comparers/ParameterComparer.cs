namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class ParameterComparer : ElementComparer<IParameterDefinition>, IParameterComparer
    {
        private readonly IParameterModifiersComparer _parameterModifiersComparer;

        public ParameterComparer(IParameterModifiersComparer parameterModifiersComparer,
            IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
            _parameterModifiersComparer = parameterModifiersComparer
                                          ?? throw new ArgumentNullException(nameof(parameterModifiersComparer));
        }

        protected override void EvaluateModifierChanges(ItemMatch<IParameterDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            base.EvaluateModifierChanges(match, options, aggregator);

            RunComparisonStep(EvaluateParameterModifierChanges, match, options, aggregator);
        }

        protected override void EvaluateSignatureChanges(ItemMatch<IParameterDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            base.EvaluateSignatureChanges(match, options, aggregator);

            RunComparisonStep(EvaluateParameterTypeChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateDefaultValueChanges, match, options, aggregator);
        }

        private static void EvaluateDefaultValueChanges(
            ItemMatch<IParameterDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldItem = match.OldItem;
            var newItem = match.NewItem;

            if (string.IsNullOrWhiteSpace(oldItem.DefaultValue)
                && string.IsNullOrWhiteSpace(newItem.DefaultValue) == false)
            {
                // A default value has been added, this is technically a feature because the consuming assembly can treat it like an overload
                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has added the default value {NewValue}",
                    match.NewItem.FullName,
                    null,
                    newItem.DefaultValue);

                aggregator.AddElementChangedResult(SemVerChangeType.Feature, match, options.MessageFormatter, args);
            }
            else if (string.IsNullOrWhiteSpace(oldItem.DefaultValue) == false
                     && string.IsNullOrWhiteSpace(newItem.DefaultValue))
            {
                // A default value has been removed
                // This will not be a breaking change for existing applications that happen to use a new binary without recompilation
                // however it does cause a breaking change for compiling existing applications against this API
                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has removed the default value {OldValue}",
                    match.NewItem.FullName,
                    oldItem.DefaultValue,
                    null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
        }

        private static void EvaluateParameterTypeChanges(
            ItemMatch<IParameterDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldType = match.OldItem.Type;
            var newType = match.NewItem.Type;

            string oldMappedType = oldType;

            if (match.OldItem.DeclaringMember is IGenericTypeElement oldGenericMember
                && match.NewItem.DeclaringMember is IGenericTypeElement newGenericMember)
            {
                oldMappedType = oldGenericMember.GetMatchingGenericType(oldType, newGenericMember);
            }
            
            if (oldMappedType != newType)
            {
                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has change type from {OldValue} to {NewValue}",
                    match.NewItem.FullName,
                    oldType,
                    newType);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
        }

        private void EvaluateParameterModifierChanges(
            ItemMatch<IParameterDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch = new ItemMatch<IModifiersElement<ParameterModifiers>>(match.OldItem, match.NewItem);

            var results = _parameterModifiersComparer.CompareMatch(convertedMatch, options);

            aggregator.AddResults(results);
        }
    }
}