namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class PropertyAccessorComparer : ElementComparer<IPropertyAccessorDefinition>, IPropertyAccessorComparer
    {
        private readonly IPropertyAccessorAccessModifierChangeTable _propertyAccessorAccessModifierChangeTable;

        public PropertyAccessorComparer(
            IPropertyAccessorAccessModifierChangeTable propertyAccessorAccessModifierChangeTable,
            IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
            _propertyAccessorAccessModifierChangeTable = propertyAccessorAccessModifierChangeTable
                                                         ?? throw new ArgumentNullException(
                                                             nameof(propertyAccessorAccessModifierChangeTable));
        }

        protected override void EvaluateMatch(ItemMatch<IPropertyAccessorDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            RunComparisonStep(EvaluateAccessModifierChanges, match, options, aggregator, true);
        }

        private void EvaluateAccessModifierChanges(
            ItemMatch<IPropertyAccessorDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var change = _propertyAccessorAccessModifierChangeTable.CalculateChange(match.OldItem.AccessModifiers,
                match.NewItem.AccessModifiers);

            if (change == SemVerChangeType.None)
            {
                return;
            }

            var newModifiers = match.NewItem.GetDeclaredAccessModifiers();
            var oldModifiers = match.OldItem.GetDeclaredAccessModifiers();

            if (string.IsNullOrWhiteSpace(oldModifiers))
            {
                // Modifiers have been added where there were previously none defined
                var suffix = string.Empty;

                if (newModifiers.Contains(" "))
                {
                    // There is more than one modifiers
                    suffix = "s";
                }

                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has added the {NewValue} access modifiers" + suffix,
                    match.NewItem.FullName, null, newModifiers);

                aggregator.AddElementChangedResult(change, match, options.MessageFormatter, args);
            }
            else if (string.IsNullOrWhiteSpace(newModifiers))
            {
                // All previous modifiers have been removed
                var suffix = string.Empty;

                if (oldModifiers.Contains(" "))
                {
                    // There is more than one modifiers
                    suffix = "s";
                }

                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has removed the {OldValue} access modifiers" + suffix,
                    match.NewItem.FullName, oldModifiers, null);

                aggregator.AddElementChangedResult(change, match, options.MessageFormatter, args);
            }
            else
            {
                // Modifiers have been changed
                var suffix = string.Empty;

                if (oldModifiers.Contains(" "))
                {
                    // There is more than one modifiers
                    suffix = "s";
                }

                var args = new FormatArguments(
                    $"{{DefinitionType}} {{Identifier}} has changed the access modifiers{suffix} from {{OldValue}} to {{NewValue}}",
                    match.NewItem.FullName, oldModifiers, newModifiers);

                aggregator.AddElementChangedResult(change, match, options.MessageFormatter, args);
            }
        }
    }
}