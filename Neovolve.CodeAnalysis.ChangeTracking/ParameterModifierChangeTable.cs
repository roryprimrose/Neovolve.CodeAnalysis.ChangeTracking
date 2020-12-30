namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class ParameterModifierChangeTable
    {
        private static readonly Dictionary<ParameterModifier, Dictionary<ParameterModifier, SemVerChangeType>>
            _modifierChanges =
                BuildModifierChanges();

        public static SemVerChangeType CalculateChange(ItemMatch<IParameterDefinition> match)
        {
            var oldModifiers = match.OldItem.Modifier;
            var newModifiers = match.NewItem.Modifier;

            if (oldModifiers == newModifiers)
            {
                // There is no change in the modifiers
                return SemVerChangeType.None;
            }

            var possibleChanges = _modifierChanges[oldModifiers];

            return possibleChanges[newModifiers];
        }

        private static void AddModifierChange(
            IDictionary<ParameterModifier, Dictionary<ParameterModifier, SemVerChangeType>> changes,
            ParameterModifier oldModifier, ParameterModifier newModifiers, SemVerChangeType changeType)
        {
            if (changes.ContainsKey(oldModifier) == false)
            {
                changes[oldModifier] = new Dictionary<ParameterModifier, SemVerChangeType>();
            }

            changes[oldModifier][newModifiers] = changeType;
        }

        private static Dictionary<ParameterModifier, Dictionary<ParameterModifier, SemVerChangeType>>
            BuildModifierChanges()
        {
            var changes = new Dictionary<ParameterModifier, Dictionary<ParameterModifier, SemVerChangeType>>();

            // @formatter:off — disable formatter after this line
            AddModifierChange(changes, ParameterModifier.None, ParameterModifier.None, SemVerChangeType.None);
            AddModifierChange(changes, ParameterModifier.None, ParameterModifier.Ref, SemVerChangeType.Breaking);
            AddModifierChange(changes, ParameterModifier.None, ParameterModifier.Out, SemVerChangeType.Breaking);
            AddModifierChange(changes, ParameterModifier.None, ParameterModifier.This, SemVerChangeType.Feature);
            AddModifierChange(changes, ParameterModifier.None, ParameterModifier.Params, SemVerChangeType.Feature);
            AddModifierChange(changes, ParameterModifier.Ref, ParameterModifier.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, ParameterModifier.Ref, ParameterModifier.Ref, SemVerChangeType.None);
            AddModifierChange(changes, ParameterModifier.Ref, ParameterModifier.Out, SemVerChangeType.Breaking);
            AddModifierChange(changes, ParameterModifier.Ref, ParameterModifier.This, SemVerChangeType.Breaking);
            AddModifierChange(changes, ParameterModifier.Ref, ParameterModifier.Params, SemVerChangeType.Breaking);
            AddModifierChange(changes, ParameterModifier.Out, ParameterModifier.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, ParameterModifier.Out, ParameterModifier.Ref, SemVerChangeType.Breaking);
            AddModifierChange(changes, ParameterModifier.Out, ParameterModifier.Out, SemVerChangeType.None);
            AddModifierChange(changes, ParameterModifier.Out, ParameterModifier.This, SemVerChangeType.Breaking);
            AddModifierChange(changes, ParameterModifier.Out, ParameterModifier.Params, SemVerChangeType.Breaking);
            AddModifierChange(changes, ParameterModifier.This, ParameterModifier.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, ParameterModifier.This, ParameterModifier.Ref, SemVerChangeType.Breaking);
            AddModifierChange(changes, ParameterModifier.This, ParameterModifier.Out, SemVerChangeType.Breaking);
            AddModifierChange(changes, ParameterModifier.This, ParameterModifier.This, SemVerChangeType.None);
            AddModifierChange(changes, ParameterModifier.This, ParameterModifier.Params, SemVerChangeType.Breaking);
            AddModifierChange(changes, ParameterModifier.Params, ParameterModifier.None, SemVerChangeType.Breaking);
            AddModifierChange(changes, ParameterModifier.Params, ParameterModifier.Ref, SemVerChangeType.Breaking);
            AddModifierChange(changes, ParameterModifier.Params, ParameterModifier.Out, SemVerChangeType.Breaking);
            AddModifierChange(changes, ParameterModifier.Params, ParameterModifier.This, SemVerChangeType.Breaking);
            AddModifierChange(changes, ParameterModifier.Params, ParameterModifier.Params, SemVerChangeType.None);
            // @formatter:on — enable formatter after this line

            return changes;
        }
    }
}