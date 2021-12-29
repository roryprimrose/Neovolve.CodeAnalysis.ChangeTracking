namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="IElementDefinition" />
    ///     interface defines the members that describe a code element.
    /// </summary>
    public interface IElementDefinition : IItemDefinition
    {
        /// <summary>
        ///     Returns whether this element matches the provided element.
        /// </summary>
        /// <param name="element">The element to evaluate.</param>
        /// <param name="options">The match options.</param>
        /// <returns>A value indicating whether the elements match.</returns>
        bool Matches(IElementDefinition element, ElementMatchOptions options);

        /// <summary>
        ///     Gets the attributes defined on the type.
        /// </summary>
        IReadOnlyCollection<IAttributeDefinition> Attributes { get; }

        /// <summary>
        ///     Gets the modifiers declared on the definition.
        /// </summary>
        string DeclaredModifiers { get; }

        /// <summary>
        ///     Gets the full name of the element including any parent hierarchy.
        /// </summary>
        string FullName { get; }

        /// <summary>
        ///     Gets the full name of the element including any parent hierarchy but without generic type parameters.
        /// </summary>
        string FullRawName { get; }

        /// <summary>
        ///     Gets whether the member is publicly visible.
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        ///     Gets the name of the element without any parent hierarchy or generic type parameters.
        /// </summary>
        string RawName { get; }
    }
}