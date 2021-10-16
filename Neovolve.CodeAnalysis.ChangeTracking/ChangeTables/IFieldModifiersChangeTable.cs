namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IFieldModifiersChangeTable"/>
    ///     interface defines the members for identifying the semantic version impact between two <see cref="FieldModifiers"/> values.
    /// </summary>
    public interface IFieldModifiersChangeTable : IChangeTable<FieldModifiers>
    {
    }
}