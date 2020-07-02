﻿namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

    public interface IElementDefinition : IItemDefinition
    {
        /// <summary>
        ///     Gets the attributes defined on the type.
        /// </summary>
        IReadOnlyCollection<IAttributeDefinition> Attributes { get; }

        /// <summary>
        ///     Gets the type that declares the member.
        /// </summary>
        public ITypeDefinition? DeclaringType { get; }

        /// <summary>
        ///     Gets the full
        /// </summary>
        string FullName { get; }

        /// <summary>
        ///     Gets whether the member is publicly visible.
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        ///     Gets the scope of the element.
        /// </summary>
        string Scope { get; }
    }
}