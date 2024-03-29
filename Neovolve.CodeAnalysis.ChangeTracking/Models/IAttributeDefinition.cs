﻿namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="IAttributeDefinition" />
    ///     interface defines the members for an attribute.
    /// </summary>
    public interface IAttributeDefinition : IItemDefinition
    {
        /// <summary>
        ///     Gets the arguments supplied to the attribute.
        /// </summary>
        IReadOnlyCollection<IArgumentDefinition> Arguments { get; }

        /// <summary>
        ///     Gets the element that declares this attribute.
        /// </summary>
        IElementDefinition DeclaringElement { get; }
    }
}