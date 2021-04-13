namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IMemberComparer{T}" />
    ///     interface defines the members for comparing members.
    /// </summary>
    /// <typeparam name="T">The type of member to compare.</typeparam>
    public interface IMemberComparer<T> : IElementComparer<T> where T : IMemberDefinition
    {
    }
}