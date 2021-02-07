namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using System;

    public interface IChangeTable<in T> where T : struct, Enum
    {
        SemVerChangeType CalculateChange(T oldValue, T newValue);
    }
}