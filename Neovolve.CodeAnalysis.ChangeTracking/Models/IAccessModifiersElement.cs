namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;

    /// <summary>
    ///     The <see cref="IAccessModifiersElement{T}" />
    ///     interface defines an element that exposes access modifiers.
    /// </summary>
    public interface IAccessModifiersElement<out T> : IElementDefinition where T : struct, Enum
    {
        /// <summary>
        ///     Gets the access modifiers of the element.
        /// </summary>
        T AccessModifiers { get; }
    }
}