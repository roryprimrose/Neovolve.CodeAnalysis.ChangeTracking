namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IMethodModifiersChangeTable"/>
    ///     interface defines the members for identifying the semantic version impact between two <see cref="MethodModifiers"/> values.
    /// </summary>
    public interface IMethodModifiersChangeTable : IChangeTable<MethodModifiers>
    {
    }
}