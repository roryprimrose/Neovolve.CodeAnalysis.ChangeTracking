namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

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