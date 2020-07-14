namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class FieldModifiersChangeTable
    {
        private static readonly Dictionary<FieldModifiers, Dictionary<FieldModifiers, SemVerChangeType>>
            _modifierChanges =
                BuildModifierChanges();

        public static SemVerChangeType CalculateChange(ItemMatch<IFieldDefinition> match)
        {
            var oldModifiers = match.OldItem.Modifiers;
            var newModifiers = match.NewItem.Modifiers;

            return CalculateChange(oldModifiers, newModifiers);
        }

        private static void AddModifierChange(
            IDictionary<FieldModifiers, Dictionary<FieldModifiers, SemVerChangeType>> changes,
            FieldModifiers oldModifier, FieldModifiers newModifiers, SemVerChangeType changeType)
        {
            if (changes.ContainsKey(oldModifier) == false)
            {
                changes[oldModifier] = new Dictionary<FieldModifiers, SemVerChangeType>();
            }

            changes[oldModifier][newModifiers] = changeType;
        }

        private static Dictionary<FieldModifiers, Dictionary<FieldModifiers, SemVerChangeType>>
            BuildModifierChanges()
        {
            var changes = new Dictionary<FieldModifiers, Dictionary<FieldModifiers, SemVerChangeType>>();

            // @formatter:off — disable formatter after this line
            AddModifierChange(changes, FieldModifiers.None, FieldModifiers.None, SemVerChangeType.None);
            AddModifierChange(changes, FieldModifiers.None, FieldModifiers.ReadOnly, SemVerChangeType.Breaking);
            AddModifierChange(changes, FieldModifiers.None, FieldModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, FieldModifiers.None, FieldModifiers.StaticReadOnly, SemVerChangeType.Breaking);
            AddModifierChange(changes, FieldModifiers.ReadOnly, FieldModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, FieldModifiers.ReadOnly, FieldModifiers.ReadOnly, SemVerChangeType.None);
            AddModifierChange(changes, FieldModifiers.ReadOnly, FieldModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, FieldModifiers.ReadOnly, FieldModifiers.StaticReadOnly, SemVerChangeType.Breaking);
            AddModifierChange(changes, FieldModifiers.Static, FieldModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, FieldModifiers.Static, FieldModifiers.ReadOnly, SemVerChangeType.Breaking);
            AddModifierChange(changes, FieldModifiers.Static, FieldModifiers.Static, SemVerChangeType.None);
            AddModifierChange(changes, FieldModifiers.Static, FieldModifiers.StaticReadOnly, SemVerChangeType.Breaking);
            AddModifierChange(changes, FieldModifiers.StaticReadOnly, FieldModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, FieldModifiers.StaticReadOnly, FieldModifiers.ReadOnly, SemVerChangeType.Breaking);
            AddModifierChange(changes, FieldModifiers.StaticReadOnly, FieldModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, FieldModifiers.StaticReadOnly, FieldModifiers.StaticReadOnly, SemVerChangeType.None);
            // @formatter:on — enable formatter after this line

            return changes;
        }

        private static SemVerChangeType CalculateChange(FieldModifiers oldModifiers, FieldModifiers newModifiers)
        {
            if (oldModifiers == newModifiers)
            {
                // There is no change in the modifiers
                return SemVerChangeType.None;
            }

            if (_modifierChanges.ContainsKey(oldModifiers) == false)
            {
                // There are no changes for this combination
                return SemVerChangeType.None;
            }

            var possibleChanges = _modifierChanges[oldModifiers];

            if (possibleChanges.ContainsKey(newModifiers) == false)
            {
                // There is no change between the modifiers
                return SemVerChangeType.None;
            }

            return possibleChanges[newModifiers];
        }
    }
}