namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class PropertyAccessorChangeTable
    {
        private static readonly Dictionary<PropertyModifiers, Dictionary<PropertyModifiers, SemVerChangeType>> _modifierChanges =
            BuildModifierChanges();

        private static Dictionary<PropertyModifiers, Dictionary<PropertyModifiers, SemVerChangeType>> BuildModifierChanges()
        {
            var changes = new Dictionary<PropertyModifiers, Dictionary<PropertyModifiers, SemVerChangeType>>();

            AddModifierChange(changes, PropertyModifiers.None, PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.None, PropertyModifiers.Override, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.None, PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.None, PropertyModifiers.Virtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.None, PropertyModifiers.New | PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.None, PropertyModifiers.New | PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.None, PropertyModifiers.New | PropertyModifiers.Virtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.Abstract, PropertyModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Abstract, PropertyModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Abstract, PropertyModifiers.Override, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.Abstract, PropertyModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Abstract, PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Abstract, PropertyModifiers.Virtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.Abstract, PropertyModifiers.New | PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Abstract, PropertyModifiers.New | PropertyModifiers.Virtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.Abstract, PropertyModifiers.Sealed | PropertyModifiers.Override,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New, PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New, PropertyModifiers.Override, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.New, PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New, PropertyModifiers.Virtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.New, PropertyModifiers.New | PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New, PropertyModifiers.New | PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New, PropertyModifiers.New | PropertyModifiers.Virtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.Override, PropertyModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Override, PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Override, PropertyModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Override, PropertyModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Override, PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Override, PropertyModifiers.New | PropertyModifiers.Abstract,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Override, PropertyModifiers.New | PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Override, PropertyModifiers.Sealed | PropertyModifiers.Override,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Sealed, PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Sealed, PropertyModifiers.Override, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.Sealed, PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Sealed, PropertyModifiers.Virtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.Sealed, PropertyModifiers.New | PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Sealed, PropertyModifiers.New | PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Sealed, PropertyModifiers.New | PropertyModifiers.Virtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.Static, PropertyModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Static, PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Static, PropertyModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Static, PropertyModifiers.Override, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Static, PropertyModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Static, PropertyModifiers.Virtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Static, PropertyModifiers.New | PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Static, PropertyModifiers.New | PropertyModifiers.Virtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Static, PropertyModifiers.Sealed | PropertyModifiers.Override,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Virtual, PropertyModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Virtual, PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Virtual, PropertyModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Virtual, PropertyModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Virtual, PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Virtual, PropertyModifiers.New | PropertyModifiers.Abstract,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Virtual, PropertyModifiers.New | PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Virtual, PropertyModifiers.Sealed | PropertyModifiers.Override,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Abstract, PropertyModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Abstract, PropertyModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Abstract, PropertyModifiers.Override,
                SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Abstract, PropertyModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Abstract, PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Abstract, PropertyModifiers.Virtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Abstract, PropertyModifiers.New | PropertyModifiers.Static,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Abstract, PropertyModifiers.New | PropertyModifiers.Virtual,
                SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Abstract, PropertyModifiers.Sealed | PropertyModifiers.Override,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Static, PropertyModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Static, PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Static, PropertyModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Static, PropertyModifiers.Override, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Static, PropertyModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Static, PropertyModifiers.Virtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Static, PropertyModifiers.New | PropertyModifiers.Abstract,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Static, PropertyModifiers.New | PropertyModifiers.Virtual,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Static, PropertyModifiers.Sealed | PropertyModifiers.Override,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Virtual, PropertyModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Virtual, PropertyModifiers.Abstract,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Virtual, PropertyModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Virtual, PropertyModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Virtual, PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Virtual, PropertyModifiers.New | PropertyModifiers.Abstract,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Virtual, PropertyModifiers.New | PropertyModifiers.Static,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New | PropertyModifiers.Virtual, PropertyModifiers.Sealed | PropertyModifiers.Override,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Sealed | PropertyModifiers.Override, PropertyModifiers.Abstract,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Sealed | PropertyModifiers.Override, PropertyModifiers.Override,
                SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.Sealed | PropertyModifiers.Override, PropertyModifiers.Static,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Sealed | PropertyModifiers.Override, PropertyModifiers.Virtual,
                SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.Sealed | PropertyModifiers.Override, PropertyModifiers.New | PropertyModifiers.Abstract,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Sealed | PropertyModifiers.Override, PropertyModifiers.New | PropertyModifiers.Static,
                SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Sealed | PropertyModifiers.Override, PropertyModifiers.New | PropertyModifiers.Virtual,
                SemVerChangeType.Feature);

            return changes;
        }

        private static void AddModifierChange(Dictionary<PropertyModifiers, Dictionary<PropertyModifiers, SemVerChangeType>> changes,
            PropertyModifiers oldModifier, PropertyModifiers newModifiers, SemVerChangeType changeType)
        {
            if (changes.ContainsKey(oldModifier) == false)
            {
                changes[oldModifier] = new Dictionary<PropertyModifiers, SemVerChangeType>();
            }

            changes[oldModifier][newModifiers] = changeType;
        }

        private static PropertyModifiers DetermineModifiers(IPropertyDefinition item)
        {
            var modifiers = PropertyModifiers.None;

            if (item.IsAbstract)
            {
                modifiers = modifiers | PropertyModifiers.Abstract;
            }

            if (item.IsOverride)
            {
                modifiers = modifiers | PropertyModifiers.Override;
            }

            if (item.IsNew)
            {
                modifiers = modifiers | PropertyModifiers.New;
            }

            if (item.IsSealed)
            {
                modifiers = modifiers | PropertyModifiers.Sealed;
            }

            if (item.IsStatic)
            {
                modifiers = modifiers | PropertyModifiers.Static;
            }

            if (item.IsVirtual)
            {
                modifiers = modifiers | PropertyModifiers.Virtual;
            }

            return modifiers;
        }

        public static SemVerChangeType CalculateChange(ItemMatch<IPropertyDefinition> match)
        {
            var oldModifiers = DetermineModifiers(match.OldItem);
            var newModifiers = DetermineModifiers(match.NewItem);

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