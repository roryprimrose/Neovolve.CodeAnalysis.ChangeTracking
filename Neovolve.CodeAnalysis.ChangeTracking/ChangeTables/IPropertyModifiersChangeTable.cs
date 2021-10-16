namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IPropertyModifiersChangeTable"/>
    ///     interface defines the members for identifying the semantic version impact between two <see cref="PropertyModifiers"/> values.
    /// </summary>
    public interface IPropertyModifiersChangeTable : IChangeTable<PropertyModifiers>
    {
    }
}