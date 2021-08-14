namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="IEnumDefinition" />
    ///     interface defines the members that describe a enum.
    /// </summary>
    public interface IEnumDefinition : IAccessModifiersElement<AccessModifiers>
    {
        /// <summary>
        ///     Gets or sets the type that declares the member.
        /// </summary>
        public ITypeDefinition? DeclaringType { get; set; }

        /// <summary>
        ///     Gets the members defined on this type.
        /// </summary>
        IReadOnlyCollection<IEnumMemberDefinition> Members { get; }

        /// <summary>
        ///     Gets the namespace of the type.
        /// </summary>
        string Namespace { get; }
    }
}