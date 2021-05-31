namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="IStructDefinition" />
    ///     interface defines the members that describe a struct.
    /// </summary>
    public interface IStructDefinition : ITypeDefinition, IModifiersElement<StructModifiers>
    {
        /// <summary>
        ///     Gets the constructors declared on the struct.
        /// </summary>
        IReadOnlyCollection<IConstructorDefinition> Constructors { get; }

        /// <summary>
        ///     Gets the fields declared on the struct.
        /// </summary>
        IReadOnlyCollection<IFieldDefinition> Fields { get; }
    }
}