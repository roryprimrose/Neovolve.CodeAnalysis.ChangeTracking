namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyAccessorAccessModifiersChangeTable : IPropertyAccessorAccessModifiersChangeTable
    {
        private static readonly Dictionary<PropertyAccessorAccessModifiers,
                Dictionary<PropertyAccessorAccessModifiers, SemVerChangeType>>
            _modifierChanges = BuildModifierChanges();

        public SemVerChangeType CalculateChange(PropertyAccessorAccessModifiers oldModifiers,
            PropertyAccessorAccessModifiers newModifiers)
        {
            if (oldModifiers == newModifiers)
            {
                // There is no change in the modifiers
                return SemVerChangeType.None;
            }
            
            var possibleChanges = _modifierChanges[oldModifiers];
            
            return possibleChanges[newModifiers];
        }

        private static void AddModifierChange(
            IDictionary<PropertyAccessorAccessModifiers, Dictionary<PropertyAccessorAccessModifiers, SemVerChangeType>>
                changes,
            PropertyAccessorAccessModifiers oldModifiers,
            PropertyAccessorAccessModifiers newModifiers,
            SemVerChangeType changeType)
        {
            if (changes.ContainsKey(oldModifiers) == false)
            {
                changes[oldModifiers] = new Dictionary<PropertyAccessorAccessModifiers, SemVerChangeType>();
            }

            changes[oldModifiers][newModifiers] = changeType;
        }

        private static Dictionary<PropertyAccessorAccessModifiers,
                Dictionary<PropertyAccessorAccessModifiers, SemVerChangeType>>
            BuildModifierChanges()
        {
            var changes =
                new Dictionary<PropertyAccessorAccessModifiers,
                    Dictionary<PropertyAccessorAccessModifiers, SemVerChangeType>>();

            // @formatter:off — disable formatter after this line
            AddModifierChange(changes, PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.None, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.Private, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.Feature );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.Feature );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.None, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.Private, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.Feature );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.Feature );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.None, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.Private, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.Feature );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.Feature );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.None, SemVerChangeType.Breaking );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.Breaking );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.Private, SemVerChangeType.Breaking );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.None, SemVerChangeType.Breaking );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.Breaking );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.Private, SemVerChangeType.Breaking );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.None );
            AddModifierChange(changes, PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.None );
            // @formatter:on — enable formatter after this line

            return changes;
        }
    }
}