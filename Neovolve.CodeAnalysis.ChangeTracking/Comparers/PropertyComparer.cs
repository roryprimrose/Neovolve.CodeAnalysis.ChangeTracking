namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class PropertyComparer : MemberComparer<IPropertyDefinition>, IPropertyComparer
    {
        private readonly IPropertyAccessorMatchProcessor _accessorProcessor;

        public PropertyComparer(
            IAccessModifiersComparer accessModifiersComparer, IPropertyAccessorMatchProcessor accessorProcessor,
            IAttributeMatchProcessor attributeProcessor) : base(accessModifiersComparer, attributeProcessor)
        {
            _accessorProcessor = accessorProcessor ?? throw new ArgumentNullException(nameof(accessorProcessor));
        }

        protected override void EvaluateMatch(
            ItemMatch<IPropertyDefinition> match,
            ComparerOptions options, IChangeResultAggregator aggregator)
        {
            RunComparisonStep(EvaluateModifierChanges, match, options, aggregator);
            RunComparisonStep(EvaluatePropertyAccessors, match, options, aggregator);

            base.EvaluateMatch(match, options, aggregator);
        }

        private static void EvaluateModifierChanges(
            ItemMatch<IPropertyDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var change = MemberModifiersChangeTable.CalculateChange(match);

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
                    // There is more than one modifiers
                    suffix = "s";
                }

                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has added the {NewValue} modifiers" + suffix,
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
                    "{DefinitionType} {Identifier} has removed the {OldValue} modifiers" + suffix,
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
                    $"{{DefinitionType}} {{Identifier}} has changed the modifiers{suffix} from {{OldValue}} to {{NewValue}}",
                    match.NewItem.FullName, oldModifiers, newModifiers);

                aggregator.AddElementChangedResult(change, match, options.MessageFormatter, args);
            }
        }

        private static IEnumerable<IPropertyAccessorDefinition> GetAccessorList(IPropertyDefinition definition)
        {
            var accessors = new List<IPropertyAccessorDefinition>();

            if (definition.GetAccessor != null)
            {
                accessors.Add(definition.GetAccessor);
            }

            if (definition.SetAccessor != null)
            {
                accessors.Add(definition.SetAccessor);
            }

            return accessors;
        }

        private void EvaluatePropertyAccessors(
            ItemMatch<IPropertyDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldAccessors = GetAccessorList(match.OldItem);
            var newAccessors = GetAccessorList(match.NewItem);

            var changes = _accessorProcessor.CalculateChanges(oldAccessors, newAccessors, options);

            aggregator.AddResults(changes);
        }
    }
}