namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class ParameterComparer : ElementComparer<IParameterDefinition>, IParameterComparer
    {
        private readonly IParameterModifiersChangeTable _parameterModifiersChangeTable;

        public ParameterComparer(IParameterModifiersChangeTable parameterModifiersChangeTable,
            IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
            _parameterModifiersChangeTable = parameterModifiersChangeTable
                                             ?? throw new ArgumentNullException(nameof(parameterModifiersChangeTable));
        }

        protected override void EvaluateMatch(
            ItemMatch<IParameterDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            RunComparisonStep(EvaluateModifierChanges, match, options, aggregator);
            RunComparisonStep(EvaluateTypeChanges, match, options, aggregator, true);
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
                // A default value has been provided, this is technically a feature because the consuming assembly can treat it like an overload
                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has added the default value {NewValue}",
                    match.NewItem.FullName,
                    null,
                    newItem.DefaultValue);

                aggregator.AddElementChangedResult(SemVerChangeType.Feature, match, options.MessageFormatter, args);
            }
        }

        private static void EvaluateTypeChanges(
            ItemMatch<IParameterDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldType = match.OldItem.Type;
            var newType = match.NewItem.Type;

            if (oldType != newType)
            {
                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has change type from {OldValue} to {NewValue}",
                    match.NewItem.FullName,
                    oldType,
                    newType);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
        }

        private void EvaluateModifierChanges(
            ItemMatch<IParameterDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var change = _parameterModifiersChangeTable.CalculateChange(match.OldItem.Modifier, match.NewItem.Modifier);

            if (change == SemVerChangeType.None)
            {
                return;
            }

            var newModifiers = match.NewItem.GetDeclaredModifiers();
            var oldModifiers = match.OldItem.GetDeclaredModifiers();

            if (string.IsNullOrWhiteSpace(oldModifiers))
            {
                // Modifiers have been added where there were previously none defined
                var suffix = string.Empty;

                if (newModifiers.Contains(" "))
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has added the {NewValue} modifier" + suffix,
                    match.NewItem.FullName,
                    null,
                    newModifiers);

                aggregator.AddElementChangedResult(change, match, options.MessageFormatter, args);
            }
            else if (string.IsNullOrWhiteSpace(newModifiers))
            {
                // All previous modifiers have been removed
                var suffix = string.Empty;

                if (oldModifiers.Contains(" "))
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has removed the {OldValue} modifier" + suffix,
                    match.NewItem.FullName,
                    oldModifiers,
                    null);

                aggregator.AddElementChangedResult(change, match, options.MessageFormatter, args);
            }
            else
            {
                // Modifiers have been changed
                var suffix = string.Empty;

                if (oldModifiers.Contains(" "))
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    $"{{DefinitionType}} {{Identifier}} has changed the modifier{suffix} from {{OldValue}} to {{NewValue}}",
                    match.NewItem.FullName,
                    oldModifiers,
                    newModifiers);

                aggregator.AddElementChangedResult(change, match, options.MessageFormatter, args);
            }
        }
    }
}