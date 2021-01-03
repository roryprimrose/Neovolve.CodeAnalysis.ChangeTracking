namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class AccessModifiersChangeTable : IAccessModifiersChangeTable
    {
        private static readonly Dictionary<AccessModifiers, Dictionary<AccessModifiers, SemVerChangeType>>
            _modifierChanges = BuildModifierChanges();

        public SemVerChangeType CalculateChange(AccessModifiers oldValue, AccessModifiers newValue)
        {
            if (oldValue == newValue)
            {
                // There is no change in the modifiers
                return SemVerChangeType.None;
            }

            var possibleChanges = _modifierChanges[oldValue];

            return possibleChanges[newValue];
        }

        private static void AddModifierChange(
            IDictionary<AccessModifiers, Dictionary<AccessModifiers, SemVerChangeType>> changes,
            AccessModifiers oldModifiers,
            AccessModifiers newModifiers,
            SemVerChangeType changeType)
        {
            if (changes.ContainsKey(oldModifiers) == false)
            {
                changes[oldModifiers] = new Dictionary<AccessModifiers, SemVerChangeType>();
            }

            changes[oldModifiers][newModifiers] = changeType;
        }

        private static Dictionary<AccessModifiers, Dictionary<AccessModifiers, SemVerChangeType>> BuildModifierChanges()
        {
            var changes = new Dictionary<AccessModifiers, Dictionary<AccessModifiers, SemVerChangeType>>();

            // @formatter:off — disable formatter after this line
            AddModifierChange(changes, AccessModifiers.Internal, AccessModifiers.Internal, SemVerChangeType.None);
            AddModifierChange(changes, AccessModifiers.Internal, AccessModifiers.Private, SemVerChangeType.None);
            AddModifierChange(changes, AccessModifiers.Internal, AccessModifiers.Protected, SemVerChangeType.Feature);
            AddModifierChange(changes, AccessModifiers.Internal, AccessModifiers.Public, SemVerChangeType.Feature);
            AddModifierChange(changes, AccessModifiers.Internal, AccessModifiers.ProtectedInternal, SemVerChangeType.Feature);
            AddModifierChange(changes, AccessModifiers.Internal, AccessModifiers.ProtectedPrivate, SemVerChangeType.Feature);
            AddModifierChange(changes, AccessModifiers.Private, AccessModifiers.Internal, SemVerChangeType.None);
            AddModifierChange(changes, AccessModifiers.Private, AccessModifiers.Private, SemVerChangeType.None);
            AddModifierChange(changes, AccessModifiers.Private, AccessModifiers.Protected, SemVerChangeType.Feature);
            AddModifierChange(changes, AccessModifiers.Private, AccessModifiers.Public, SemVerChangeType.Feature);
            AddModifierChange(changes, AccessModifiers.Private, AccessModifiers.ProtectedInternal, SemVerChangeType.Feature);
            AddModifierChange(changes, AccessModifiers.Private, AccessModifiers.ProtectedPrivate, SemVerChangeType.Feature);
            AddModifierChange(changes, AccessModifiers.Protected, AccessModifiers.Internal, SemVerChangeType.Breaking);
            AddModifierChange(changes, AccessModifiers.Protected, AccessModifiers.Private, SemVerChangeType.Breaking);
            AddModifierChange(changes, AccessModifiers.Protected, AccessModifiers.Protected, SemVerChangeType.None);
            AddModifierChange(changes, AccessModifiers.Protected, AccessModifiers.Public, SemVerChangeType.Feature);
            AddModifierChange(changes, AccessModifiers.Protected, AccessModifiers.ProtectedInternal, SemVerChangeType.None);
            AddModifierChange(changes, AccessModifiers.Protected, AccessModifiers.ProtectedPrivate, SemVerChangeType.None);
            AddModifierChange(changes, AccessModifiers.Public, AccessModifiers.Internal, SemVerChangeType.Breaking);
            AddModifierChange(changes, AccessModifiers.Public, AccessModifiers.Private, SemVerChangeType.Breaking);
            AddModifierChange(changes, AccessModifiers.Public, AccessModifiers.Protected, SemVerChangeType.Breaking);
            AddModifierChange(changes, AccessModifiers.Public, AccessModifiers.Public, SemVerChangeType.None);
            AddModifierChange(changes, AccessModifiers.Public, AccessModifiers.ProtectedInternal, SemVerChangeType.Breaking);
            AddModifierChange(changes, AccessModifiers.Public, AccessModifiers.ProtectedPrivate, SemVerChangeType.Breaking);
            AddModifierChange(changes, AccessModifiers.ProtectedInternal, AccessModifiers.Internal, SemVerChangeType.Breaking);
            AddModifierChange(changes, AccessModifiers.ProtectedInternal, AccessModifiers.Private, SemVerChangeType.Breaking);
            AddModifierChange(changes, AccessModifiers.ProtectedInternal, AccessModifiers.Protected, SemVerChangeType.None);
            AddModifierChange(changes, AccessModifiers.ProtectedInternal, AccessModifiers.Public, SemVerChangeType.Feature);
            AddModifierChange(changes, AccessModifiers.ProtectedInternal, AccessModifiers.ProtectedInternal, SemVerChangeType.None);
            AddModifierChange(changes, AccessModifiers.ProtectedInternal, AccessModifiers.ProtectedPrivate, SemVerChangeType.None);
            AddModifierChange(changes, AccessModifiers.ProtectedPrivate, AccessModifiers.Internal, SemVerChangeType.Breaking);
            AddModifierChange(changes, AccessModifiers.ProtectedPrivate, AccessModifiers.Private, SemVerChangeType.Breaking);
            AddModifierChange(changes, AccessModifiers.ProtectedPrivate, AccessModifiers.Protected, SemVerChangeType.None);
            AddModifierChange(changes, AccessModifiers.ProtectedPrivate, AccessModifiers.Public, SemVerChangeType.Feature);
            AddModifierChange(changes, AccessModifiers.ProtectedPrivate, AccessModifiers.ProtectedInternal, SemVerChangeType.None);
            AddModifierChange(changes, AccessModifiers.ProtectedPrivate, AccessModifiers.ProtectedPrivate, SemVerChangeType.None);
            // @formatter:on — enable formatter after this line

            return changes;
        }
    }
}