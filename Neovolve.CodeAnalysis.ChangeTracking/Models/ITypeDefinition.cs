namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="ITypeDefinition" />
    ///     interface defines common properties for types.
    /// </summary>
    public interface ITypeDefinition : IAccessModifiersElement<AccessModifiers>, IGenericTypeElement
    {
        /// <summary>
        ///     Gets the child classes defined on this type.
        /// </summary>
        IReadOnlyCollection<IClassDefinition> ChildClasses { get; }

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
        IReadOnlyCollection<ITypeDefinition> ChildTypes { get; }

        /// <summary>
        ///     Gets the type that declares the member.
        /// </summary>
        public ITypeDefinition? DeclaringType { get; }
        
        /// <summary>
        ///     Gets the types implemented/inherited by this type.
        /// </summary>
        IReadOnlyCollection<string> ImplementedTypes { get; }

        /// <summary>
        ///     Gets the methods declared on the type.
        /// </summary>
        IReadOnlyCollection<IMethodDefinition> Methods { get; }

        /// <summary>
        ///     Gets the namespace of the type.
        /// </summary>
        string Namespace { get; set; }

        /// <summary>
        ///     Gets the properties declared on the type.
        /// </summary>
        IReadOnlyCollection<IPropertyDefinition> Properties { get; }
    }
}