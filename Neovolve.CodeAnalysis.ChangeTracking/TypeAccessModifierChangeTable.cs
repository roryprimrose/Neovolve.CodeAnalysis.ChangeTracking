namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class TypeAccessModifierChangeTable
    {
        private static readonly Dictionary<TypeAccessModifier, Dictionary<TypeAccessModifier, SemVerChangeType>>
            _modifierChanges = BuildModifierChanges();

        public static SemVerChangeType CalculateChange(ItemMatch<ITypeDefinition> match)
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
            IDictionary<TypeAccessModifier, Dictionary<TypeAccessModifier, SemVerChangeType>> changes,
            TypeAccessModifier oldModifier,
            TypeAccessModifier newModifiers,
            SemVerChangeType changeType)
        {
            if (changes.ContainsKey(oldModifier) == false)
            {
                changes[oldModifier] = new Dictionary<TypeAccessModifier, SemVerChangeType>();
            }

            changes[oldModifier][newModifiers] = changeType;
        }

        private static Dictionary<TypeAccessModifier, Dictionary<TypeAccessModifier, SemVerChangeType>>
            BuildModifierChanges()
        {
            var changes = new Dictionary<TypeAccessModifier, Dictionary<TypeAccessModifier, SemVerChangeType>>();

            // @formatter:off — disable formatter after this line

            AddModifierChange(changes, TypeAccessModifier.None, TypeAccessModifier.None, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.None, TypeAccessModifier.Internal, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.None, TypeAccessModifier.Private, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.None, TypeAccessModifier.Protected, SemVerChangeType.Feature );
            AddModifierChange(changes, TypeAccessModifier.None, TypeAccessModifier.Public, SemVerChangeType.Feature );
            AddModifierChange(changes, TypeAccessModifier.None, TypeAccessModifier.ProtectedInternal, SemVerChangeType.Feature );
            AddModifierChange(changes, TypeAccessModifier.None, TypeAccessModifier.InternalPrivate, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.Internal, TypeAccessModifier.None, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.Internal, TypeAccessModifier.Internal, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.Internal, TypeAccessModifier.Private, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.Internal, TypeAccessModifier.Protected, SemVerChangeType.Feature );
            AddModifierChange(changes, TypeAccessModifier.Internal, TypeAccessModifier.Public, SemVerChangeType.Feature );
            AddModifierChange(changes, TypeAccessModifier.Internal, TypeAccessModifier.ProtectedInternal, SemVerChangeType.Feature );
            AddModifierChange(changes, TypeAccessModifier.Internal, TypeAccessModifier.InternalPrivate, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.Private, TypeAccessModifier.None, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.Private, TypeAccessModifier.Internal, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.Private, TypeAccessModifier.Private, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.Private, TypeAccessModifier.Protected, SemVerChangeType.Feature );
            AddModifierChange(changes, TypeAccessModifier.Private, TypeAccessModifier.Public, SemVerChangeType.Feature );
            AddModifierChange(changes, TypeAccessModifier.Private, TypeAccessModifier.ProtectedInternal, SemVerChangeType.Feature );
            AddModifierChange(changes, TypeAccessModifier.Private, TypeAccessModifier.InternalPrivate, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.Protected, TypeAccessModifier.None, SemVerChangeType.Breaking );
            AddModifierChange(changes, TypeAccessModifier.Protected, TypeAccessModifier.Internal, SemVerChangeType.Breaking );
            AddModifierChange(changes, TypeAccessModifier.Protected, TypeAccessModifier.Private, SemVerChangeType.Breaking );
            AddModifierChange(changes, TypeAccessModifier.Protected, TypeAccessModifier.Protected, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.Protected, TypeAccessModifier.Public, SemVerChangeType.Feature );
            AddModifierChange(changes, TypeAccessModifier.Protected, TypeAccessModifier.ProtectedInternal, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.Protected, TypeAccessModifier.InternalPrivate, SemVerChangeType.Breaking );
            AddModifierChange(changes, TypeAccessModifier.Public, TypeAccessModifier.None, SemVerChangeType.Breaking );
            AddModifierChange(changes, TypeAccessModifier.Public, TypeAccessModifier.Internal, SemVerChangeType.Breaking );
            AddModifierChange(changes, TypeAccessModifier.Public, TypeAccessModifier.Private, SemVerChangeType.Breaking );
            AddModifierChange(changes, TypeAccessModifier.Public, TypeAccessModifier.Protected, SemVerChangeType.Breaking );
            AddModifierChange(changes, TypeAccessModifier.Public, TypeAccessModifier.Public, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.Public, TypeAccessModifier.ProtectedInternal, SemVerChangeType.Breaking );
            AddModifierChange(changes, TypeAccessModifier.Public, TypeAccessModifier.InternalPrivate, SemVerChangeType.Breaking );
            AddModifierChange(changes, TypeAccessModifier.ProtectedInternal, TypeAccessModifier.None, SemVerChangeType.Breaking );
            AddModifierChange(changes, TypeAccessModifier.ProtectedInternal, TypeAccessModifier.Internal, SemVerChangeType.Breaking );
            AddModifierChange(changes, TypeAccessModifier.ProtectedInternal, TypeAccessModifier.Private, SemVerChangeType.Breaking );
            AddModifierChange(changes, TypeAccessModifier.ProtectedInternal, TypeAccessModifier.Protected, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.ProtectedInternal, TypeAccessModifier.Public, SemVerChangeType.Feature );
            AddModifierChange(changes, TypeAccessModifier.ProtectedInternal, TypeAccessModifier.ProtectedInternal, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.ProtectedInternal, TypeAccessModifier.InternalPrivate, SemVerChangeType.Breaking );
            AddModifierChange(changes, TypeAccessModifier.InternalPrivate, TypeAccessModifier.None, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.InternalPrivate, TypeAccessModifier.Internal, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.InternalPrivate, TypeAccessModifier.Private, SemVerChangeType.None );
            AddModifierChange(changes, TypeAccessModifier.InternalPrivate, TypeAccessModifier.Protected, SemVerChangeType.Feature );
            AddModifierChange(changes, TypeAccessModifier.InternalPrivate, TypeAccessModifier.Public, SemVerChangeType.Feature );
            AddModifierChange(changes, TypeAccessModifier.InternalPrivate, TypeAccessModifier.ProtectedInternal, SemVerChangeType.Feature );
            AddModifierChange(changes, TypeAccessModifier.InternalPrivate, TypeAccessModifier.InternalPrivate, SemVerChangeType.None );
            // @formatter:on — enable formatter after this line

            return changes;
        }

        private static TypeAccessModifier DetermineModifiers(IElementDefinition item)
        {
            if (item.AccessModifiers == "internal")
            {
                return TypeAccessModifier.Internal;
            }

            if (item.AccessModifiers == "protected")
            {
                return TypeAccessModifier.Protected;
            }

            if (item.AccessModifiers == "private")
            {
                return TypeAccessModifier.Private;
            }

            if (item.AccessModifiers == "public")
            {
                return TypeAccessModifier.Public;
            }

            if (item.AccessModifiers.Contains("internal")
                && item.AccessModifiers.Contains("private"))
            {
                return TypeAccessModifier.InternalPrivate;
            }

            if (item.AccessModifiers.Contains("protected")
                && item.AccessModifiers.Contains("internal"))
            {
                return TypeAccessModifier.ProtectedInternal;
            }

            return TypeAccessModifier.None;
        }
    }
}