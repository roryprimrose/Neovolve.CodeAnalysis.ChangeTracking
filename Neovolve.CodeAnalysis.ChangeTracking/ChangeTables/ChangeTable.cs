namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using System;
    using System.Collections.Generic;

    public abstract class ChangeTable<T> : IChangeTable<T> where T : struct, Enum
    {
        private readonly Dictionary<T, Dictionary<T, SemVerChangeType>> _changes = new Dictionary<T, Dictionary<T, SemVerChangeType>>();
        
        protected abstract void BuildChanges();
        
        protected void AddChange(
            T oldModifiers,
            T newModifiers,
            SemVerChangeType changeType)
        {
            if (oldModifiers.Equals(newModifiers))
            {
                throw new InvalidOperationException(
                    "The values stored are the same. Do not add a change with the same values as it will consume unnecessary memory.");
            }

            if (_changes.ContainsKey(oldModifiers) == false)
            {
                _changes[oldModifiers] = new Dictionary<T, SemVerChangeType>();
            }

            _changes[oldModifiers][newModifiers] = changeType;
        }

        public SemVerChangeType CalculateChange(T oldValue, T newValue)
        {
            if (oldValue.Equals(newValue))
            {
                // There is no change in the modifiers
                return SemVerChangeType.None;
            }
            
            if (_changes.Count == 0)
            {
                BuildChanges();
            }

            if (_changes.ContainsKey(oldValue))
            {
                var possibleChanges = _changes[oldValue];

                if (possibleChanges.ContainsKey(newValue))
                {
                    return possibleChanges[newValue];
                }
            }

            throw new InvalidOperationException(
                $"There is no {typeof(T).Name} {nameof(SemVerChangeType)} recorded for comparing {oldValue} with {newValue}");
        }
    }
}