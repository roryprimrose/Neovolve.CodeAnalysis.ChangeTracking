namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="IMemberDefinition" />
    ///     interface defines common properties for members.
    /// </summary>
    public interface IMemberDefinition : IItemDefinition
    {
        /// <summary>
        ///     Gets the attributes defined on the type.
        /// </summary>
        public IReadOnlyCollection<AttributeDefinition> Attributes { get; }

        /// <summary>
        ///     Gets the full
        /// </summary>
        public string FullName { get; }

        /// <summary>
        ///     Gets whether the member is publicly visible.
        /// </summary>
        public bool IsVisible { get; }

        /// <summary>
        ///     Gets the type that declares the member.
        /// </summary>
        public ITypeDefinition? DeclaringType { get; }
    }
}