namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="IConstructorDefinition" />
    ///     interface defines the members that describe a constructor.
    /// </summary>
    public interface IConstructorDefinition : IMemberDefinition, IModifiersElement<ConstructorModifiers>
    {
        /// <summary>
        ///     Gets the parameters defined on this constructor.
        /// </summary>
        public IReadOnlyCollection<IParameterDefinition> Parameters { get; }
    }
}