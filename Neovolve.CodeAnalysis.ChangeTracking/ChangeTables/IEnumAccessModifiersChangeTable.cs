namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IEnumAccessModifiersChangeTable"/>
    ///     interface defines the members for identifying the semantic version impact between two <see cref="EnumAccessModifiers"/> values.
    /// </summary>
    public interface IEnumAccessModifiersChangeTable : IChangeTable<EnumAccessModifiers>
    {

    }
}