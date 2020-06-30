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
    public abstract class TypeDefinition : ITypeDefinition
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

            DeclaringType = null;
            Namespace = node.DetermineNamespace();
            Name = DetermineName(node);
            FullName = Namespace + "." + Name;

            Attributes = node.DetermineAttributes(this);
            IsVisible = node.IsVisible();
            ImplementedTypes = DetermineImplementedTypes(node);
            GenericConstraints = Array.Empty<ConstraintListDefinition>();
            Location = node.DetermineLocation();
            Properties = DetermineProperties(node);
            ChildClasses = DetermineChildClasses(node);
            ChildInterfaces = DetermineChildInterfaces(node);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeDefinition" /> class.
        /// </summary>
        /// <param name="declaringType">The parent type that declares this type.</param>
        /// <param name="node">The syntax node that defines the type.</param>
        protected TypeDefinition(TypeDefinition declaringType, BaseTypeDeclarationSyntax node)
        {
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));

            Namespace = node.DetermineNamespace();
            Name = DetermineName(node);
            FullName = DeclaringType.FullName + "+" + Name;

            Attributes = node.DetermineAttributes(this);
            IsVisible = node.IsVisible();
            ImplementedTypes = DetermineImplementedTypes(node);
            GenericConstraints = Array.Empty<ConstraintListDefinition>();
            Location = node.DetermineLocation();
            Properties = DetermineProperties(node);
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

        private IReadOnlyCollection<PropertyDefinition> DetermineProperties(SyntaxNode node)
        {
            var childNodes = node.ChildNodes().OfType<PropertyDeclarationSyntax>();
            var childTypes = childNodes.Select(childNode => new PropertyDefinition(this, childNode)).ToList();

            return childTypes.AsReadOnly();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<AttributeDefinition> Attributes { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<TypeDefinition> ChildClasses { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<TypeDefinition> ChildInterfaces { get; }

        /// <inheritdoc />
        public ITypeDefinition? DeclaringType { get; }

        /// <inheritdoc />
        public string FullName { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<ConstraintListDefinition> GenericConstraints { get; protected set; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> ImplementedTypes { get; }

        /// <inheritdoc />
        public bool IsVisible { get; }

        /// <inheritdoc />
        public DefinitionLocation Location { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Namespace { get; set; }

        /// <inheritdoc />
        public IReadOnlyCollection<PropertyDefinition> Properties { get; }
    }
}