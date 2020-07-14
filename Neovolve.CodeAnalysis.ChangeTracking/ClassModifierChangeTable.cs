namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class ClassModifierChangeTable
    {
        private static readonly Dictionary<ClassModifiers, Dictionary<ClassModifiers, SemVerChangeType>>
            _modifierChanges =
                BuildModifierChanges();

        public static SemVerChangeType CalculateChange(ItemMatch<IClassDefinition> match)
        {
            var oldModifiers = match.OldItem.Modifiers;
            var newModifiers = match.NewItem.Modifiers;

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

        private static void AddModifierChange(
            IDictionary<ClassModifiers, Dictionary<ClassModifiers, SemVerChangeType>> changes,
            ClassModifiers oldModifier, ClassModifiers newModifiers, SemVerChangeType changeType)
        {
            if (changes.ContainsKey(oldModifier) == false)
            {
                changes[oldModifier] = new Dictionary<ClassModifiers, SemVerChangeType>();
            }

            changes[oldModifier][newModifiers] = changeType;
        }

        private static Dictionary<ClassModifiers, Dictionary<ClassModifiers, SemVerChangeType>>
            BuildModifierChanges()
        {
            var changes = new Dictionary<ClassModifiers, Dictionary<ClassModifiers, SemVerChangeType>>();

            // @formatter:off — disable formatter after this line
            AddModifierChange(changes, ClassModifiers.None, ClassModifiers.None, SemVerChangeType.None);
            AddModifierChange(changes, ClassModifiers.None, ClassModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.None, ClassModifiers.Partial, SemVerChangeType.None);
            AddModifierChange(changes, ClassModifiers.None, ClassModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.None, ClassModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.None, ClassModifiers.AbstractPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.None, ClassModifiers.StaticPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.None, ClassModifiers.SealedPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Abstract, ClassModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Abstract, ClassModifiers.Abstract, SemVerChangeType.None);
            AddModifierChange(changes, ClassModifiers.Abstract, ClassModifiers.Partial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Abstract, ClassModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Abstract, ClassModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Abstract, ClassModifiers.AbstractPartial, SemVerChangeType.None);
            AddModifierChange(changes, ClassModifiers.Abstract, ClassModifiers.StaticPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Abstract, ClassModifiers.SealedPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Partial, ClassModifiers.None, SemVerChangeType.None);
            AddModifierChange(changes, ClassModifiers.Partial, ClassModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Partial, ClassModifiers.Partial, SemVerChangeType.None);
            AddModifierChange(changes, ClassModifiers.Partial, ClassModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Partial, ClassModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Partial, ClassModifiers.AbstractPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Partial, ClassModifiers.StaticPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Partial, ClassModifiers.SealedPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Sealed, ClassModifiers.None, SemVerChangeType.Feature);
            AddModifierChange(changes, ClassModifiers.Sealed, ClassModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Sealed, ClassModifiers.Partial, SemVerChangeType.Feature);
            AddModifierChange(changes, ClassModifiers.Sealed, ClassModifiers.Sealed, SemVerChangeType.None);
            AddModifierChange(changes, ClassModifiers.Sealed, ClassModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Sealed, ClassModifiers.AbstractPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Sealed, ClassModifiers.StaticPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Sealed, ClassModifiers.SealedPartial, SemVerChangeType.None);
            AddModifierChange(changes, ClassModifiers.Static, ClassModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Static, ClassModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Static, ClassModifiers.Partial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Static, ClassModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Static, ClassModifiers.Static, SemVerChangeType.None);
            AddModifierChange(changes, ClassModifiers.Static, ClassModifiers.AbstractPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.Static, ClassModifiers.StaticPartial, SemVerChangeType.None);
            AddModifierChange(changes, ClassModifiers.Static, ClassModifiers.SealedPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.AbstractPartial, ClassModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.AbstractPartial, ClassModifiers.Abstract, SemVerChangeType.None);
            AddModifierChange(changes, ClassModifiers.AbstractPartial, ClassModifiers.Partial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.AbstractPartial, ClassModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.AbstractPartial, ClassModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.AbstractPartial, ClassModifiers.AbstractPartial, SemVerChangeType.None);
            AddModifierChange(changes, ClassModifiers.AbstractPartial, ClassModifiers.StaticPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.AbstractPartial, ClassModifiers.SealedPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.StaticPartial, ClassModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.StaticPartial, ClassModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.StaticPartial, ClassModifiers.Partial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.StaticPartial, ClassModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.StaticPartial, ClassModifiers.Static, SemVerChangeType.None);
            AddModifierChange(changes, ClassModifiers.StaticPartial, ClassModifiers.AbstractPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.StaticPartial, ClassModifiers.StaticPartial, SemVerChangeType.None);
            AddModifierChange(changes, ClassModifiers.StaticPartial, ClassModifiers.SealedPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.SealedPartial, ClassModifiers.None, SemVerChangeType.Feature);
            AddModifierChange(changes, ClassModifiers.SealedPartial, ClassModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.SealedPartial, ClassModifiers.Partial, SemVerChangeType.Feature);
            AddModifierChange(changes, ClassModifiers.SealedPartial, ClassModifiers.Sealed, SemVerChangeType.None);
            AddModifierChange(changes, ClassModifiers.SealedPartial, ClassModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.SealedPartial, ClassModifiers.AbstractPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.SealedPartial, ClassModifiers.StaticPartial, SemVerChangeType.Breaking);
            AddModifierChange(changes, ClassModifiers.SealedPartial, ClassModifiers.SealedPartial, SemVerChangeType.None);
            // @formatter:on — enable formatter after this line

            return changes;
        }
    }
}