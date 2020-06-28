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
    public abstract class TypeDefinition : IItemDefinition
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
            Attributes = node.DetermineAttributes(this);
            IsVisible = node.IsVisible();
            Name = DetermineName(node, null);
            ImplementedTypes = DetermineImplementedTypes(node);
            ChildClasses = DetermineChildClasses(node);
            ChildInterfaces = DetermineChildInterfaces(node);
            Location = node.DetermineLocation();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeDefinition" /> class.
        /// </summary>
        /// <param name="parentType">The parent type that declares this type.</param>
        /// <param name="node">The syntax node that defines the type.</param>
        protected TypeDefinition(TypeDefinition parentType, BaseTypeDeclarationSyntax node)
        {
            ParentType = parentType ?? throw new ArgumentNullException(nameof(parentType));

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            Namespace = parentType.Namespace;
            Attributes = node.DetermineAttributes(this);
            IsVisible = parentType.IsVisible && node.IsVisible();
            Name = DetermineName(node, parentType);
            ImplementedTypes = DetermineImplementedTypes(node);
            ChildClasses = DetermineChildClasses(node);
            ChildInterfaces = DetermineChildInterfaces(node);
            Location = node.DetermineLocation();
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

        private static string DetermineName(BaseTypeDeclarationSyntax node, TypeDefinition? parentType)
        {
            var name = string.Empty;

            if (parentType != null)
            {
                name = parentType.Name + "+";
            }

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

        /// <summary>
        ///     Gets the attributes defined on the type.
        /// </summary>
        public IReadOnlyCollection<AttributeDefinition> Attributes { get; }

        /// <summary>
        ///     Gets the child classes defined on this type.
        /// </summary>
        public IReadOnlyCollection<TypeDefinition> ChildClasses { get; }

        /// <summary>
        ///     Gets the child interfaces defined on this type.
        /// </summary>
        public IReadOnlyCollection<TypeDefinition> ChildInterfaces { get; }

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