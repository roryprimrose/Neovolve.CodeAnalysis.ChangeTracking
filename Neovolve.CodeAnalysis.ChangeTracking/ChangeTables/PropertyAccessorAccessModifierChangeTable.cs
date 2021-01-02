namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class PropertyAccessorAccessModifierChangeTable
    {
        private static readonly Dictionary<PropertyAccessorAccessModifier, Dictionary<PropertyAccessorAccessModifier, SemVerChangeType>>
            _modifierChanges = BuildModifierChanges();

        public static SemVerChangeType CalculateChange<T>(ItemMatch<T> match) where T : IPropertyAccessorDefinition
        {
            var oldModifier = match.OldItem.AccessModifier;
            var newModifier = match.NewItem.AccessModifier;

            return CalculateChange(oldModifier, newModifier);
        }

        private static void AddModifierChange(
            IDictionary<PropertyAccessorAccessModifier, Dictionary<PropertyAccessorAccessModifier, SemVerChangeType>> changes,
            PropertyAccessorAccessModifier oldModifier,
            PropertyAccessorAccessModifier newModifier,
            SemVerChangeType changeType)
        {
            if (changes.ContainsKey(oldModifier) == false)
            {
                changes[oldModifier] = new Dictionary<PropertyAccessorAccessModifier, SemVerChangeType>();
            }

            changes[oldModifier][newModifier] = changeType;
        }

        private static Dictionary<PropertyAccessorAccessModifier, Dictionary<PropertyAccessorAccessModifier, SemVerChangeType>>
            BuildModifierChanges()
        {
            var changes = new Dictionary<PropertyAccessorAccessModifier, Dictionary<PropertyAccessorAccessModifier, SemVerChangeType>>();

            // @formatter:off — disable formatter after this line
            AddModifierChange(changes, PropertyAccessorAccessModifier.None, PropertyAccessorAccessModifier.None, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifier.None, PropertyAccessorAccessModifier.Internal, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifier.None, PropertyAccessorAccessModifier.Private, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifier.None, PropertyAccessorAccessModifier.Protected, SemVerChangeType.Feature );
            AddModifierChange(changes, PropertyAccessorAccessModifier.None, PropertyAccessorAccessModifier.ProtectedInternal, SemVerChangeType.Feature );
            AddModifierChange(changes, PropertyAccessorAccessModifier.Internal, PropertyAccessorAccessModifier.None, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifier.Internal, PropertyAccessorAccessModifier.Internal, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifier.Internal, PropertyAccessorAccessModifier.Private, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifier.Internal, PropertyAccessorAccessModifier.Protected, SemVerChangeType.Feature );
            AddModifierChange(changes, PropertyAccessorAccessModifier.Internal, PropertyAccessorAccessModifier.ProtectedInternal, SemVerChangeType.Feature );
            AddModifierChange(changes, PropertyAccessorAccessModifier.Private, PropertyAccessorAccessModifier.None, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifier.Private, PropertyAccessorAccessModifier.Internal, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifier.Private, PropertyAccessorAccessModifier.Private, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifier.Private, PropertyAccessorAccessModifier.Protected, SemVerChangeType.Feature );
            AddModifierChange(changes, PropertyAccessorAccessModifier.Private, PropertyAccessorAccessModifier.ProtectedInternal, SemVerChangeType.Feature );
            AddModifierChange(changes, PropertyAccessorAccessModifier.Protected, PropertyAccessorAccessModifier.None, SemVerChangeType.Breaking );
            AddModifierChange(changes, PropertyAccessorAccessModifier.Protected, PropertyAccessorAccessModifier.Internal, SemVerChangeType.Breaking );
            AddModifierChange(changes, PropertyAccessorAccessModifier.Protected, PropertyAccessorAccessModifier.Private, SemVerChangeType.Breaking );
            AddModifierChange(changes, PropertyAccessorAccessModifier.Protected, PropertyAccessorAccessModifier.Protected, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifier.Protected, PropertyAccessorAccessModifier.ProtectedInternal, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifier.ProtectedInternal, PropertyAccessorAccessModifier.None, SemVerChangeType.Breaking );
            AddModifierChange(changes, PropertyAccessorAccessModifier.ProtectedInternal, PropertyAccessorAccessModifier.Internal, SemVerChangeType.Breaking );
            AddModifierChange(changes, PropertyAccessorAccessModifier.ProtectedInternal, PropertyAccessorAccessModifier.Private, SemVerChangeType.Breaking );
            AddModifierChange(changes, PropertyAccessorAccessModifier.ProtectedInternal, PropertyAccessorAccessModifier.Protected, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifier.ProtectedInternal, PropertyAccessorAccessModifier.ProtectedInternal, SemVerChangeType.None );
            // @formatter:on — enable formatter after this line

            return changes;
        }

        private static SemVerChangeType CalculateChange(PropertyAccessorAccessModifier oldModifier, PropertyAccessorAccessModifier newModifier)
        {
            if (oldModifier == newModifier)
            {
                // There is no change in the modifiers
                return SemVerChangeType.None;
            }

            if (_modifierChanges.ContainsKey(oldModifier) == false)
            {
                // There are no changes for this combination
                return SemVerChangeType.None;
            }

            var possibleChanges = _modifierChanges[oldModifier];

            if (possibleChanges.ContainsKey(newModifier) == false)
            {
                // There is no change between the modifiers
                return SemVerChangeType.None;
            }

            return possibleChanges[newModifier];
        }
    }
}