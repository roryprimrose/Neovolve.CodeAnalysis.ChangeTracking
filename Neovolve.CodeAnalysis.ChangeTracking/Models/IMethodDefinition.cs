namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

    public interface IMethodDefinition : IMemberDefinition, IModifiersElement<MethodModifiers>, IGenericTypeElement
    {
        /// <summary>
        ///     Gets the parameters declared on the method.
        /// </summary>
        IReadOnlyCollection<IParameterDefinition> Parameters { get; }
    }
}