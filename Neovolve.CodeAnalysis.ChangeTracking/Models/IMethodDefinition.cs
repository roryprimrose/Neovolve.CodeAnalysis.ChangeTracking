namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

    public interface IMethodDefinition : IMemberDefinition
    {
        /// <summary>
        ///     Gets the generic constraints declared on the method.
        /// </summary>
        IReadOnlyCollection<IConstraintListDefinition> GenericConstraints { get; }

        /// <summary>
        ///     Gets the generic type parameters declared on the method.
        /// </summary>
        IReadOnlyCollection<string> GenericTypeParameters { get; }

        /// <summary>
        ///     Gets the modifiers of the method.
        /// </summary>
        MethodModifiers Modifiers { get; }
    }
}