namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="IGenericTypeElement" />
    ///     interface defines an element that exposes generic type parameters and constraints.
    /// </summary>
    public interface IGenericTypeElement : IElementDefinition
    {
        /// <summary>
        ///     Gets the generic constraints declared on the method.
        /// </summary>
        IReadOnlyCollection<IConstraintListDefinition> GenericConstraints { get; }

        /// <summary>
        ///     Gets the generic type parameters declared on the method.
        /// </summary>
        IReadOnlyCollection<string> GenericTypeParameters { get; }

    }
}