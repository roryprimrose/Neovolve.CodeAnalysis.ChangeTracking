namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IAccessModifiersComparer" />
    ///     interface defines the members for comparing <see cref="IAccessModifiersElementComparer{T}"/> items.
    /// </summary>
    public interface IAccessModifiersComparer : IAccessModifiersElementComparer<AccessModifiers>
    {
    }
}