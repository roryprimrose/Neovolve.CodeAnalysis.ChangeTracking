namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    public interface IChangeTable<in T> where T : notnull
    {
        SemVerChangeType CalculateChange(T oldValue, T newValue);
    }
}