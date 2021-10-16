namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IFieldComparer" />
    ///     interface defines the members for comparing <see cref="IFieldDefinition"/> items.
    /// </summary>
    public interface IFieldComparer : IMemberComparer<IFieldDefinition>
    {
    }
}