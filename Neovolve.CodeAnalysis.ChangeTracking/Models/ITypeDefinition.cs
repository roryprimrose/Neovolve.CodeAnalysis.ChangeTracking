namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="ITypeDefinition" />
    ///     interface defines common properties for types.
    /// </summary>
    public interface ITypeDefinition : IElementDefinition
    {
        /// <summary>
        ///     Gets the child classes defined on this type.
        /// </summary>
        public IReadOnlyCollection<IClassDefinition> ChildClasses { get; }

        /// <summary>
        ///     Gets the child interfaces defined on this type.
        /// </summary>
        public IReadOnlyCollection<IInterfaceDefinition> ChildInterfaces { get; }

        /// <summary>
        ///     Gets the child types defined on this type.
        /// </summary>
        /// <remarks>This should be a combination of child classes and child interfaces.</remarks>
        public IReadOnlyCollection<ITypeDefinition> ChildTypes { get; }

        /// <summary>
        ///     Gets the generic constraints declared on the type.
        /// </summary>
        public IReadOnlyCollection<IConstraintListDefinition> GenericConstraints { get; }

        /// <summary>
        ///     Gets the types implemented/inherited by this type.
        /// </summary>
        public IReadOnlyCollection<string> ImplementedTypes { get; }

        /// <summary>
        ///     Gets the namespace of the type.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        ///     Gets the properties declared on the type.
        /// </summary>
        public IReadOnlyCollection<IPropertyDefinition> Properties { get; }
    }
}