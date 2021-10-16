namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IStructModifiersChangeTable"/>
    ///     interface defines the members for identifying the semantic version impact between two <see cref="StructModifiers"/> values.
    /// </summary>
    public interface IStructModifiersChangeTable : IChangeTable<StructModifiers>
    {
    }
}