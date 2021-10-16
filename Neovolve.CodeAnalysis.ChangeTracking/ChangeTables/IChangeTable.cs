namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    /// <summary>
    ///     The <see cref="IChangeTable{T}" />
    ///     interface defines the members for identifying the semantic version impact between two values.
    /// </summary>
    /// <typeparam name="T">The type of value to evaluate.</typeparam>
    public interface IChangeTable<in T> where T : notnull
    {
        SemVerChangeType CalculateChange(T oldValue, T newValue);
    }
}