namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class StructModifierChangeTable
    {
        private static readonly Dictionary<StructModifiers, Dictionary<StructModifiers, SemVerChangeType>>
            _modifierChanges =
                BuildModifierChanges();

        public static SemVerChangeType CalculateChange(ItemMatch<IStructDefinition> match)
        {
            var oldModifiers = match.OldItem.Modifiers;
            var newModifiers = match.NewItem.Modifiers;

            if (oldModifiers == newModifiers)
            {
                // There is no change in the modifiers
                return SemVerChangeType.None;
            }

            var possibleChanges = _modifierChanges[oldModifiers];

            return possibleChanges[newModifiers];
        }

        private static void AddModifierChange(
            IDictionary<StructModifiers, Dictionary<StructModifiers, SemVerChangeType>> changes,
            StructModifiers oldModifier, StructModifiers newModifiers, SemVerChangeType changeType)
        {
            if (changes.ContainsKey(oldModifier) == false)
            {
                changes[oldModifier] = new Dictionary<StructModifiers, SemVerChangeType>();
            }

            changes[oldModifier][newModifiers] = changeType;
        }

        private static Dictionary<StructModifiers, Dictionary<StructModifiers, SemVerChangeType>>
            BuildModifierChanges()
        {
            var changes = new Dictionary<StructModifiers, Dictionary<StructModifiers, SemVerChangeType>>();

            // @formatter:off — disable formatter after this line
            AddModifierChange(changes, StructModifiers.None, StructModifiers.None, SemVerChangeType.None);
            AddModifierChange(changes, StructModifiers.None, StructModifiers.ReadOnly, SemVerChangeType.Breaking);
            AddModifierChange(changes, StructModifiers.None, StructModifiers.Partial, SemVerChangeType.None);
            AddModifierChange(changes, StructModifiers.None, StructModifiers.ReadOnlyPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, StructModifiers.ReadOnly, StructModifiers.None, SemVerChangeType.Feature);
            AddModifierChange(changes, StructModifiers.ReadOnly, StructModifiers.ReadOnly, SemVerChangeType.None);
            AddModifierChange(changes, StructModifiers.ReadOnly, StructModifiers.Partial, SemVerChangeType.Feature);
            AddModifierChange(changes, StructModifiers.ReadOnly, StructModifiers.ReadOnlyPartial, SemVerChangeType.None);
            AddModifierChange(changes, StructModifiers.Partial, StructModifiers.None, SemVerChangeType.None);
            AddModifierChange(changes, StructModifiers.Partial, StructModifiers.ReadOnly, SemVerChangeType.Breaking);
            AddModifierChange(changes, StructModifiers.Partial, StructModifiers.Partial, SemVerChangeType.None);
            AddModifierChange(changes, StructModifiers.Partial, StructModifiers.ReadOnlyPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, StructModifiers.ReadOnlyPartial, StructModifiers.None, SemVerChangeType.Feature);
            AddModifierChange(changes, StructModifiers.ReadOnlyPartial, StructModifiers.ReadOnly, SemVerChangeType.None);
            AddModifierChange(changes, StructModifiers.ReadOnlyPartial, StructModifiers.Partial, SemVerChangeType.Feature);
            AddModifierChange(changes, StructModifiers.ReadOnlyPartial, StructModifiers.ReadOnlyPartial, SemVerChangeType.None);
            // @formatter:on — enable formatter after this line

            return changes;
        }
    }
}