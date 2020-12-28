namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class MethodComparer : MemberComparer<IMethodDefinition>, IMethodComparer
    {
        public MethodComparer(IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
        }

        protected override void EvaluateMatch(
            ItemMatch<IMethodDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            RunComparisonStep(EvaluateMethodModifierChanges, match, options, aggregator);
        }

        private static void EvaluateMethodModifierChanges(
            ItemMatch<IMethodDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var change = MethodModifiersChangeTable.CalculateChange(match);

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