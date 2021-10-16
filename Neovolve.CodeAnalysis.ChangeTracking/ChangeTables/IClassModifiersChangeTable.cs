namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IClassModifiersChangeTable"/>
    ///     interface defines the members for identifying the semantic version impact between two <see cref="ClassModifiers"/> values.
    /// </summary>
    public interface IClassModifiersChangeTable : IChangeTable<ClassModifiers>
    {
    }
}