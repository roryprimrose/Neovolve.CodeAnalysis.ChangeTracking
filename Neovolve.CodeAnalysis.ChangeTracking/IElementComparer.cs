namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IElementComparer{T}" />
    ///     interface defines the members for comparing elements.
    /// </summary>
    /// <typeparam name="T">The type of element to compare.</typeparam>
    public interface IElementComparer<T> : IItemComparer<T> where T : IElementDefinition
    {
    }
}