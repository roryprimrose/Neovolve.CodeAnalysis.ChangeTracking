namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

    public interface IBaseTypeDefinition : IElementDefinition
    {
        /// <summary>
        ///     Gets or sets the type that declares the member.
        /// </summary>
        ITypeDefinition? DeclaringType { get; set; }

        /// <summary>
        ///     Gets the types implemented/inherited by this type.
        /// </summary>
        IReadOnlyCollection<string> ImplementedTypes { get; }

        /// <summary>
        ///     Gets the namespace of the type.
        /// </summary>
        string Namespace { get; }
    }
}