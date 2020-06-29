namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="TypeDefinition" />
    ///     class is used to describe a type.
    /// </summary>
    public abstract class TypeDefinition : IMemberDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeDefinition" /> class.
        /// </summary>
        /// <param name="node">The syntax node that defines the type.</param>
        protected TypeDefinition(BaseTypeDeclarationSyntax node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            ParentType = null;
            Namespace = node.DetermineNamespace();
            Name = DetermineName(node);
            FullName = Namespace + "." + Name;

            Attributes = node.DetermineAttributes(this);
            IsVisible = node.IsVisible();
            ImplementedTypes = DetermineImplementedTypes(node);
            GenericConstraints = Array.Empty<ConstraintListDefinition>();
            Location = node.DetermineLocation();
            ChildClasses = DetermineChildClasses(node);
            ChildInterfaces = DetermineChildInterfaces(node);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeDefinition" /> class.
        /// </summary>
        /// <param name="parentType">The parent type that declares this type.</param>
        /// <param name="node">The syntax node that defines the type.</param>
        protected TypeDefinition(TypeDefinition parentType, BaseTypeDeclarationSyntax node)
        {
            ParentType = parentType ?? throw new ArgumentNullException(nameof(parentType));

            Namespace = node.DetermineNamespace();
            Name = DetermineName(node);
            FullName = ParentType.FullName + "+" + Name;

            Attributes = node.DetermineAttributes(this);
            IsVisible = node.IsVisible();
            ImplementedTypes = DetermineImplementedTypes(node);
            GenericConstraints = Array.Empty<ConstraintListDefinition>();
            Location = node.DetermineLocation();
            ChildClasses = DetermineChildClasses(node);
            ChildInterfaces = DetermineChildInterfaces(node);
        }

        private static IReadOnlyCollection<string> DetermineImplementedTypes(BaseTypeDeclarationSyntax node)
        {
            var baseList = node.ChildNodes().OfType<BaseListSyntax>().FirstOrDefault();

            if (baseList == null)
            {
                return Array.Empty<string>();
            }

            var childTypes = baseList.Types.Select(x => x.ToString()).ToList();

            return childTypes.AsReadOnly();
        }

        private static string DetermineName(BaseTypeDeclarationSyntax node)
        {
            var name = string.Empty;

            name += node.Identifier.Text;

            var typeParameters = node.ChildNodes().OfType<TypeParameterListSyntax>().FirstOrDefault();

            if (typeParameters == null)
            {
                return name;
            }

            var parameterList = typeParameters.ToString();

            return name + parameterList;
        }

        private IReadOnlyCollection<ClassDefinition> DetermineChildClasses(SyntaxNode node)
        {
            var childNodes = node.ChildNodes().OfType<ClassDeclarationSyntax>();
            var childTypes = childNodes.Select(childNode => new ClassDefinition(this, childNode)).ToList();

            return childTypes.AsReadOnly();
        }

        private IReadOnlyCollection<InterfaceDefinition> DetermineChildInterfaces(SyntaxNode node)
        {
            var childNodes = node.ChildNodes().OfType<InterfaceDeclarationSyntax>();
            var childTypes = childNodes.Select(childNode => new InterfaceDefinition(this, childNode)).ToList();

            return childTypes.AsReadOnly();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<AttributeDefinition> Attributes { get; }

        /// <summary>
        ///     Gets the child classes defined on this type.
        /// </summary>
        public IReadOnlyCollection<TypeDefinition> ChildClasses { get; }

        /// <summary>
        ///     Gets the child interfaces defined on this type.
        /// </summary>
        public IReadOnlyCollection<TypeDefinition> ChildInterfaces { get; }

        /// <inheritdoc />
        public string FullName { get; }

        /// <summary>
        ///     Gets the generic constraints declared on the type.
        /// </summary>
        public IReadOnlyCollection<ConstraintListDefinition> GenericConstraints { get; protected set; }

        /// <summary>
        ///     Gets the types implemented/inherited by this type.
        /// </summary>
        public IReadOnlyCollection<string> ImplementedTypes { get; }

        /// <summary>
        ///     Gets whether the type is visible.
        /// </summary>
        public bool IsVisible { get; }

        /// <inheritdoc />
        public DefinitionLocation Location { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        ///     Gets the namespace of the type.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        ///     Gets the parent type definition where one is declared.
        /// </summary>
        public TypeDefinition? ParentType { get; }
    }
}