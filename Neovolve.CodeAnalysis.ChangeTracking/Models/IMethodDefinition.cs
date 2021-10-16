namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="IMethodDefinition" />
    ///     interface defines the members that describe a method.
    /// </summary>
    public interface IMethodDefinition : IMemberDefinition, IModifiersElement<MethodModifiers>, IGenericTypeElement
    {
        /// <summary>
        ///     Gets the parameters declared on the method.
        /// </summary>
        IReadOnlyCollection<IParameterDefinition> Parameters { get; }

        /// <summary>
        /// Gets whether this method has a body defined for it.
        /// </summary>
        public bool HasBody { get; }
    }
}