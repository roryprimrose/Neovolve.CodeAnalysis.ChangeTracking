namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IBaseTypeComparer{T}" />
    ///     interface defines the members for comparing <see cref="IBaseTypeDefinition{T}"/> items.
    /// </summary>
    /// <typeparam name="T">The type of item to compare.</typeparam>
    public interface IBaseTypeComparer<T> : IElementComparer<T> where T : IBaseTypeDefinition
    {
    }
}