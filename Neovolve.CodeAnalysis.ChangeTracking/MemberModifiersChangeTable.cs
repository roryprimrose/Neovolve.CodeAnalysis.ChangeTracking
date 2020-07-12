namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class MemberModifiersChangeTable
    {
        private static readonly Dictionary<MemberModifiers, Dictionary<MemberModifiers, SemVerChangeType>>
            _modifierChanges =
                BuildModifierChanges();

        public static SemVerChangeType CalculateChange(ItemMatch<IPropertyDefinition> match)
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
            IDictionary<MemberModifiers, Dictionary<MemberModifiers, SemVerChangeType>> changes,
            MemberModifiers oldModifier, MemberModifiers newModifiers, SemVerChangeType changeType)
        {
            if (changes.ContainsKey(oldModifier) == false)
            {
                changes[oldModifier] = new Dictionary<MemberModifiers, SemVerChangeType>();
            }

            changes[oldModifier][newModifiers] = changeType;
        }

        private static Dictionary<MemberModifiers, Dictionary<MemberModifiers, SemVerChangeType>>
            BuildModifierChanges()
        {
            var changes = new Dictionary<MemberModifiers, Dictionary<MemberModifiers, SemVerChangeType>>();

            // @formatter:off — disable formatter after this line
            AddModifierChange(changes, MemberModifiers.None, MemberModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.None, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.None, MemberModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.None, MemberModifiers.Override, SemVerChangeType.Feature);
            AddModifierChange(changes, MemberModifiers.None, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.None, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.None, MemberModifiers.Virtual, SemVerChangeType.Feature);
            AddModifierChange(changes, MemberModifiers.None, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.None, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.None, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.None, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.None, MemberModifiers.NewVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.None, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Abstract, MemberModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Abstract, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Abstract, MemberModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Abstract, MemberModifiers.Override, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Abstract, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Abstract, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Abstract, MemberModifiers.Virtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Abstract, MemberModifiers.AbstractOverride, SemVerChangeType.None);
            AddModifierChange(changes, MemberModifiers.Abstract, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Abstract, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Abstract, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Abstract, MemberModifiers.NewVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Abstract, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.New, MemberModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.New, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.New, MemberModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.New, MemberModifiers.Override, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.New, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.New, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.New, MemberModifiers.Virtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.New, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.New, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.New, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.New, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.New, MemberModifiers.NewVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.New, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Override, MemberModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Override, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Override, MemberModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Override, MemberModifiers.Override, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Override, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Override, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Override, MemberModifiers.Virtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Override, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Override, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Override, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Override, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Override, MemberModifiers.NewVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Override, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Sealed, MemberModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Sealed, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Sealed, MemberModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Sealed, MemberModifiers.Override, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Sealed, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Sealed, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Sealed, MemberModifiers.Virtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Sealed, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Sealed, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Sealed, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Sealed, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Sealed, MemberModifiers.NewVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Sealed, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Static, MemberModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Static, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Static, MemberModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Static, MemberModifiers.Override, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Static, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Static, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Static, MemberModifiers.Virtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Static, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Static, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Static, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Static, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Static, MemberModifiers.NewVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Static, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Virtual, MemberModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Virtual, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Virtual, MemberModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Virtual, MemberModifiers.Override, SemVerChangeType.None);
            AddModifierChange(changes, MemberModifiers.Virtual, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Virtual, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Virtual, MemberModifiers.Virtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Virtual, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Virtual, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Virtual, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Virtual, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Virtual, MemberModifiers.NewVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.Virtual, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.AbstractOverride, MemberModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.AbstractOverride, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.AbstractOverride, MemberModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.AbstractOverride, MemberModifiers.Override, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.AbstractOverride, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.AbstractOverride, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.AbstractOverride, MemberModifiers.Virtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.AbstractOverride, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.AbstractOverride, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.AbstractOverride, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.AbstractOverride, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.AbstractOverride, MemberModifiers.NewVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.AbstractOverride, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstract, MemberModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstract, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstract, MemberModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstract, MemberModifiers.Override, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstract, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstract, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstract, MemberModifiers.Virtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstract, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstract, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstract, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Feature);
            AddModifierChange(changes, MemberModifiers.NewAbstract, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstract, MemberModifiers.NewVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstract, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstractVirtual, MemberModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstractVirtual, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstractVirtual, MemberModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstractVirtual, MemberModifiers.Override, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstractVirtual, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstractVirtual, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstractVirtual, MemberModifiers.Virtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstractVirtual, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstractVirtual, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstractVirtual, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstractVirtual, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstractVirtual, MemberModifiers.NewVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewAbstractVirtual, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewStatic, MemberModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewStatic, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewStatic, MemberModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewStatic, MemberModifiers.Override, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewStatic, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewStatic, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewStatic, MemberModifiers.Virtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewStatic, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewStatic, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewStatic, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewStatic, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewStatic, MemberModifiers.NewVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewStatic, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewVirtual, MemberModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewVirtual, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewVirtual, MemberModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewVirtual, MemberModifiers.Override, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewVirtual, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewVirtual, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewVirtual, MemberModifiers.Virtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewVirtual, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewVirtual, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewVirtual, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewVirtual, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewVirtual, MemberModifiers.NewVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.NewVirtual, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.SealedOverride, MemberModifiers.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.SealedOverride, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.SealedOverride, MemberModifiers.New, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.SealedOverride, MemberModifiers.Override, SemVerChangeType.Feature);
            AddModifierChange(changes, MemberModifiers.SealedOverride, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.SealedOverride, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.SealedOverride, MemberModifiers.Virtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.SealedOverride, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.SealedOverride, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.SealedOverride, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.SealedOverride, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.SealedOverride, MemberModifiers.NewVirtual, SemVerChangeType.Breaking);
            AddModifierChange(changes, MemberModifiers.SealedOverride, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            // @formatter:on — enable formatter after this line

            return changes;
        }
    }
}