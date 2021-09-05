namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public abstract class ModifiersComparerBase<T> where T : struct, Enum
    {
        private readonly IChangeTable<T> _changeTable;

        protected ModifiersComparerBase(IChangeTable<T> changeTable)
        {
            _changeTable = changeTable ?? throw new ArgumentNullException(nameof(changeTable));
        }

        protected virtual IEnumerable<ComparisonResult> CompareMatch(
            ItemMatch<IElementDefinition> match,
            T oldValue,
            T newValue,
            ComparerOptions options)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));

            var change = _changeTable.CalculateChange(oldValue, newValue);

            if (change == SemVerChangeType.None)
            {
                yield break;
            }

            var newModifiers = GetDeclaredModifiers(match.NewItem);
            var oldModifiers = GetDeclaredModifiers(match.OldItem);

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
                    $"has added the {{NewValue}} {ModifierLabel}{suffix}",
                    match.NewItem.FullName,
                    null,
                    newModifiers);

                var message = options.MessageFormatter.FormatMatch(match, ItemFormatType.ItemChanged, args);

                var result = new ComparisonResult(
                    change,
                    match.OldItem, match.NewItem,
                    message);

                yield return result;
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
                    $"has removed the {{OldValue}} {ModifierLabel}{suffix}",
                    match.NewItem.FullName,
                    oldModifiers,
                    null);

                var message = options.MessageFormatter.FormatMatch(match, ItemFormatType.ItemChanged, args);

                var result = new ComparisonResult(
                    change,
                    match.OldItem, match.NewItem,
                    message);

                yield return result;
            }
            else
            {
                // Modifiers have been changed
                var suffix = string.Empty;

                if (oldModifiers.Contains(" ")
                    || newModifiers.Contains(" "))
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    $"has changed the {ModifierLabel}{suffix} from {{OldValue}} to {{NewValue}}",
                    match.NewItem.FullName,
                    oldModifiers,
                    newModifiers);

                var message = options.MessageFormatter.FormatMatch(match, ItemFormatType.ItemChanged, args);

                var result = new ComparisonResult(
                    change,
                    match.OldItem, match.NewItem,
                    message);

                yield return result;
            }
        }

        protected abstract string GetDeclaredModifiers(IElementDefinition element);

        protected abstract string ModifierLabel { get; }
    }
}