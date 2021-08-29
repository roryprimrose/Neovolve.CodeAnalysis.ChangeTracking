namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="ITypeDefinition" />
    ///     interface defines common properties for types.
    /// </summary>
    public interface ITypeDefinition : IBaseTypeDefinition<AccessModifiers>, IGenericTypeElement
    {
        /// <summary>
        ///     Merges the partial type into this type.
        /// </summary>
        /// <param name="partialType">The partial type to merge.</param>
        void MergePartialType(ITypeDefinition partialType);

        /// <summary>
        ///     Gets the child classes defined on this type.
        /// </summary>
        IReadOnlyCollection<IClassDefinition> ChildClasses { get; }

        /// <summary>
        ///     Gets the child enums defined on this type.
        /// </summary>
        IReadOnlyCollection<IEnumDefinition> ChildEnums { get; }

        /// <summary>
        ///     Gets the child interfaces defined on this type.
        /// </summary>
        IReadOnlyCollection<IInterfaceDefinition> ChildInterfaces { get; }

        /// <summary>
        ///     Gets the child structs defined on this type.
        /// </summary>
        public IReadOnlyCollection<IStructDefinition> ChildStructs { get; }

        /// <summary>
        ///     Gets the child types defined on this type.
        /// </summary>
        /// <remarks>This should be a combination of child classes and child interfaces.</remarks>
        IReadOnlyCollection<IBaseTypeDefinition> ChildTypes { get; }

        /// <summary>
        ///     Gets the methods declared on the type.
        /// </summary>
        IReadOnlyCollection<IMethodDefinition> Methods { get; }

        /// <summary>
        ///     Gets the properties declared on the type.
        /// </summary>
        IReadOnlyCollection<IPropertyDefinition> Properties { get; }
    }
}