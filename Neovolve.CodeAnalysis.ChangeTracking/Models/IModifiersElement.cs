namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;

    /// <summary>
    ///     The <see cref="IModifiersElement{T}" />
    ///     interface defines an element that exposes modifiers.
    /// </summary>
    public interface IModifiersElement<out T> : IElementDefinition where T : struct, Enum
    {
        /// <summary>
        ///     Gets the modifiers of the element.
        /// </summary>
        T Modifiers { get; }
    }
}