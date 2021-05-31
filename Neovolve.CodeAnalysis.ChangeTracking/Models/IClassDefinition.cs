namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="IClassDefinition" />
    ///     interface defines the members that describe a class.
    /// </summary>
    public interface IClassDefinition : ITypeDefinition, IModifiersElement<ClassModifiers>
    {
        /// <summary>
        ///     Gets the constructors declared on the class.
        /// </summary>
        IReadOnlyCollection<IConstructorDefinition> Constructors { get; }

        /// <summary>
        ///     Gets the fields declared on the class.
        /// </summary>
        IReadOnlyCollection<IFieldDefinition> Fields { get; }
    }
}