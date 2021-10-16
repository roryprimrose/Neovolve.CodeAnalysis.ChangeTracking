namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IParameterModifiersChangeTable"/>
    ///     interface defines the members for identifying the semantic version impact between two <see cref="ParameterModifiers"/> values.
    /// </summary>
    public interface IParameterModifiersChangeTable : IChangeTable<ParameterModifiers>
    {
    }
}