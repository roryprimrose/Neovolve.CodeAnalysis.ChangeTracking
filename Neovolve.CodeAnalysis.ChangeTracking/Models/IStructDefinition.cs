namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="IStructDefinition" />
    ///     interface defines the members that describe a class.
    /// </summary>
    public interface IStructDefinition : ITypeDefinition, IModifiersElement<StructModifiers>
    {
        /// <summary>
        ///     Gets the fields declared on the class.
        /// </summary>
        IReadOnlyCollection<IFieldDefinition> Fields { get; }
    }
}