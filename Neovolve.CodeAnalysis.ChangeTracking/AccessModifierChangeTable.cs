namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class AccessModifierChangeTable
    {
        private static readonly Dictionary<AccessModifier, Dictionary<AccessModifier, SemVerChangeType>>
            _modifierChanges = BuildModifierChanges();

        public static SemVerChangeType CalculateChange<T>(ItemMatch<T> match) where T : IMemberDefinition
        {
            var oldModifier = match.OldItem.AccessModifier;
            var newModifier = match.NewItem.AccessModifier;

            return CalculateChange(oldModifier, newModifier);
        }

        public static SemVerChangeType CalculateChange(ItemMatch<ITypeDefinition> match)
        {
            var oldModifier = match.OldItem.AccessModifier;
            var newModifier = match.NewItem.AccessModifier;

            return CalculateChange(oldModifier, newModifier);
        }

        private static void AddModifierChange(
            IDictionary<AccessModifier, Dictionary<AccessModifier, SemVerChangeType>> changes,
            AccessModifier oldModifier,
            AccessModifier newModifier,
            SemVerChangeType changeType)
        {
            if (changes.ContainsKey(oldModifier) == false)
            {
                changes[oldModifier] = new Dictionary<AccessModifier, SemVerChangeType>();
            }

            changes[oldModifier][newModifier] = changeType;
        }

        private static Dictionary<AccessModifier, Dictionary<AccessModifier, SemVerChangeType>>
            BuildModifierChanges()
        {
            var changes = new Dictionary<AccessModifier, Dictionary<AccessModifier, SemVerChangeType>>();

            // @formatter:off — disable formatter after this line
            AddModifierChange(changes, AccessModifier.Internal, AccessModifier.Internal, SemVerChangeType.None );
            AddModifierChange(changes, AccessModifier.Internal, AccessModifier.Private, SemVerChangeType.None );
            AddModifierChange(changes, AccessModifier.Internal, AccessModifier.Protected, SemVerChangeType.Feature );
            AddModifierChange(changes, AccessModifier.Internal, AccessModifier.Public, SemVerChangeType.Feature );
            AddModifierChange(changes, AccessModifier.Internal, AccessModifier.ProtectedInternal, SemVerChangeType.Feature );
            AddModifierChange(changes, AccessModifier.Internal, AccessModifier.ProtectedPrivate, SemVerChangeType.Feature );
            AddModifierChange(changes, AccessModifier.Private, AccessModifier.Internal, SemVerChangeType.None );
            AddModifierChange(changes, AccessModifier.Private, AccessModifier.Private, SemVerChangeType.None );
            AddModifierChange(changes, AccessModifier.Private, AccessModifier.Protected, SemVerChangeType.Feature );
            AddModifierChange(changes, AccessModifier.Private, AccessModifier.Public, SemVerChangeType.Feature );
            AddModifierChange(changes, AccessModifier.Private, AccessModifier.ProtectedInternal, SemVerChangeType.Feature );
            AddModifierChange(changes, AccessModifier.Private, AccessModifier.ProtectedPrivate, SemVerChangeType.Feature );
            AddModifierChange(changes, AccessModifier.Protected, AccessModifier.Internal, SemVerChangeType.Breaking );
            AddModifierChange(changes, AccessModifier.Protected, AccessModifier.Private, SemVerChangeType.Breaking );
            AddModifierChange(changes, AccessModifier.Protected, AccessModifier.Protected, SemVerChangeType.None );
            AddModifierChange(changes, AccessModifier.Protected, AccessModifier.Public, SemVerChangeType.Feature );
            AddModifierChange(changes, AccessModifier.Protected, AccessModifier.ProtectedInternal, SemVerChangeType.None );
            AddModifierChange(changes, AccessModifier.Protected, AccessModifier.ProtectedPrivate, SemVerChangeType.None );
            AddModifierChange(changes, AccessModifier.Public, AccessModifier.Internal, SemVerChangeType.Breaking );
            AddModifierChange(changes, AccessModifier.Public, AccessModifier.Private, SemVerChangeType.Breaking );
            AddModifierChange(changes, AccessModifier.Public, AccessModifier.Protected, SemVerChangeType.Breaking );
            AddModifierChange(changes, AccessModifier.Public, AccessModifier.Public, SemVerChangeType.None );
            AddModifierChange(changes, AccessModifier.Public, AccessModifier.ProtectedInternal, SemVerChangeType.Breaking );
            AddModifierChange(changes, AccessModifier.Public, AccessModifier.ProtectedPrivate, SemVerChangeType.Breaking );
            AddModifierChange(changes, AccessModifier.ProtectedInternal, AccessModifier.Internal, SemVerChangeType.Breaking );
            AddModifierChange(changes, AccessModifier.ProtectedInternal, AccessModifier.Private, SemVerChangeType.Breaking );
            AddModifierChange(changes, AccessModifier.ProtectedInternal, AccessModifier.Protected, SemVerChangeType.None );
            AddModifierChange(changes, AccessModifier.ProtectedInternal, AccessModifier.Public, SemVerChangeType.Feature );
            AddModifierChange(changes, AccessModifier.ProtectedInternal, AccessModifier.ProtectedInternal, SemVerChangeType.None );
            AddModifierChange(changes, AccessModifier.ProtectedInternal, AccessModifier.ProtectedPrivate, SemVerChangeType.None );
            AddModifierChange(changes, AccessModifier.ProtectedPrivate, AccessModifier.Internal, SemVerChangeType.Breaking);
            AddModifierChange(changes, AccessModifier.ProtectedPrivate, AccessModifier.Private, SemVerChangeType.Breaking );
            AddModifierChange(changes, AccessModifier.ProtectedPrivate, AccessModifier.Protected, SemVerChangeType.None );
            AddModifierChange(changes, AccessModifier.ProtectedPrivate, AccessModifier.Public, SemVerChangeType.Feature );
            AddModifierChange(changes, AccessModifier.ProtectedPrivate, AccessModifier.ProtectedInternal, SemVerChangeType.None );
            AddModifierChange(changes, AccessModifier.ProtectedPrivate, AccessModifier.ProtectedPrivate, SemVerChangeType.None );
            // @formatter:on — enable formatter after this line

            return changes;
        }

        private static SemVerChangeType CalculateChange(AccessModifier oldModifier, AccessModifier newModifier)
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