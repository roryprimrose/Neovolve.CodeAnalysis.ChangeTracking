namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IAccessModifiersChangeTable"/>
    ///     interface defines the members for identifying the semantic version impact between two <see cref="AccessModifiers"/> values.
    /// </summary>
    public interface IAccessModifiersChangeTable : IChangeTable<AccessModifiers>
    {

    }
}