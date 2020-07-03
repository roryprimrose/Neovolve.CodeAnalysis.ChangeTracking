namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class PropertyModifierChangeTable
    {
        private static readonly Dictionary<PropertyModifiers, Dictionary<PropertyModifiers, SemVerChangeType>>
            _modifierChanges =
                BuildModifierChanges();

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

        private static void AddModifierChange(
            Dictionary<PropertyModifiers, Dictionary<PropertyModifiers, SemVerChangeType>> changes,
            PropertyModifiers oldModifier, PropertyModifiers newModifiers, SemVerChangeType changeType)
        {
            if (changes.ContainsKey(oldModifier) == false)
            {
                changes[oldModifier] = new Dictionary<PropertyModifiers, SemVerChangeType>();
            }

            changes[oldModifier][newModifiers] = changeType;
        }

        private static Dictionary<PropertyModifiers, Dictionary<PropertyModifiers, SemVerChangeType>>
            BuildModifierChanges()
        {
            var changes = new Dictionary<PropertyModifiers, Dictionary<PropertyModifiers, SemVerChangeType>>();

            // @formatter:off — disable formatter after this line
            AddModifierChange(changes, PropertyModifiers.None, PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.None, PropertyModifiers.Override, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.None, PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.None, PropertyModifiers.Virtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.None, PropertyModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.None, PropertyModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.None, PropertyModifiers.NewVirtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.Abstract, PropertyModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Abstract, PropertyModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Abstract, PropertyModifiers.Override, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.Abstract, PropertyModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Abstract, PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Abstract, PropertyModifiers.Virtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.Abstract, PropertyModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Abstract, PropertyModifiers.NewVirtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.Abstract, PropertyModifiers.SealedVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New, PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New, PropertyModifiers.Override, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.New, PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New, PropertyModifiers.Virtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.New, PropertyModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New, PropertyModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.New, PropertyModifiers.NewVirtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.Override, PropertyModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Override, PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Override, PropertyModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Override, PropertyModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Override, PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Override, PropertyModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Override, PropertyModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Override, PropertyModifiers.SealedVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Sealed, PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Sealed, PropertyModifiers.Override, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.Sealed, PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Sealed, PropertyModifiers.Virtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.Sealed, PropertyModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Sealed, PropertyModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Sealed, PropertyModifiers.NewVirtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.Static, PropertyModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Static, PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Static, PropertyModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Static, PropertyModifiers.Override, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Static, PropertyModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Static, PropertyModifiers.Virtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Static, PropertyModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Static, PropertyModifiers.NewVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Static, PropertyModifiers.SealedVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Virtual, PropertyModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Virtual, PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Virtual, PropertyModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Virtual, PropertyModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Virtual, PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Virtual, PropertyModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Virtual, PropertyModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.Virtual, PropertyModifiers.SealedVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewAbstract, PropertyModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewAbstract, PropertyModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewAbstract, PropertyModifiers.Override, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.NewAbstract, PropertyModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewAbstract, PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewAbstract, PropertyModifiers.Virtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.NewAbstract, PropertyModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewAbstract, PropertyModifiers.NewVirtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.NewAbstract, PropertyModifiers.SealedVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewStatic, PropertyModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewStatic, PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewStatic, PropertyModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewStatic, PropertyModifiers.Override, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewStatic, PropertyModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewStatic, PropertyModifiers.Virtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewStatic, PropertyModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewStatic, PropertyModifiers.NewVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewStatic, PropertyModifiers.SealedVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewVirtual, PropertyModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewVirtual, PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewVirtual, PropertyModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewVirtual, PropertyModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewVirtual, PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewVirtual, PropertyModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewVirtual, PropertyModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.NewVirtual, PropertyModifiers.SealedVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.SealedVirtual, PropertyModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.SealedVirtual, PropertyModifiers.Override, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.SealedVirtual, PropertyModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.SealedVirtual, PropertyModifiers.Virtual, SemVerChangeType.Feature);
            AddModifierChange(changes, PropertyModifiers.SealedVirtual, PropertyModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.SealedVirtual, PropertyModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, PropertyModifiers.SealedVirtual, PropertyModifiers.NewVirtual, SemVerChangeType.Feature);
            // @formatter:on — enable formatter after this line

            return changes;
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
    }
}