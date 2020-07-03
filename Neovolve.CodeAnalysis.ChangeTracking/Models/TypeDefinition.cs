namespace Neovolve.CodeAnalysis.ChangeTracking.Models
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
        protected TypeDefinition(TypeDeclarationSyntax node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            Location = node.DetermineLocation();
            DeclaringType = null;
            Namespace = node.DetermineNamespace();
            Scope = node.DetermineScope();
            Name = DetermineName(node);
            RawName = node.Identifier.Text;
            FullRawName = Namespace + "." + RawName;
            FullName = Namespace + "." + Name;

            IsVisible = node.IsVisible();
            ImplementedTypes = DetermineImplementedTypes(node);
            Attributes = node.DetermineAttributes(this);
            Properties = DetermineProperties(node);
            ChildClasses = DetermineChildClasses(node);
            ChildInterfaces = DetermineChildInterfaces(node);
            ChildTypes = DetermineChildTypes(ChildClasses, ChildInterfaces);
            GenericTypeParameters = DetermineGenericTypeParameters(node);
            GenericConstraints = DetermineGenericConstraints(node);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeDefinition" /> class.
        /// </summary>
        /// <param name="declaringType">The parent type that declares this type.</param>
        /// <param name="node">The syntax node that defines the type.</param>
        protected TypeDefinition(ITypeDefinition declaringType, TypeDeclarationSyntax node)
        {
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));

            Location = node.DetermineLocation();
            Namespace = node.DetermineNamespace();
            Scope = node.DetermineScope();
            Name = DetermineName(node);
            RawName = node.Identifier.Text;
            FullRawName = DeclaringType.FullRawName + "+" + RawName;
            FullName = DeclaringType.FullName + "+" + Name;

            Attributes = node.DetermineAttributes(this);
            IsVisible = node.IsVisible();
            ImplementedTypes = DetermineImplementedTypes(node);
            Properties = DetermineProperties(node);
            ChildClasses = DetermineChildClasses(node);
            ChildInterfaces = DetermineChildInterfaces(node);
            ChildTypes = DetermineChildTypes(ChildClasses, ChildInterfaces);
            GenericTypeParameters = DetermineGenericTypeParameters(node);
            GenericConstraints = DetermineGenericConstraints(node);
        }

        private static IReadOnlyCollection<ITypeDefinition> DetermineChildTypes(
            IReadOnlyCollection<ITypeDefinition> childClasses,
            IReadOnlyCollection<ITypeDefinition> childInterfaces)
        {
            var childTypes = new List<ITypeDefinition>(childClasses);

            childTypes.AddRange(childInterfaces);

            return childTypes;
        }

        private static IReadOnlyCollection<IConstraintListDefinition> DetermineGenericConstraints(
            TypeDeclarationSyntax node)
        {
            var constraintLists = new List<ConstraintListDefinition>();

            foreach (var clauses in node.ConstraintClauses)
            {
                var constraintList = new ConstraintListDefinition(clauses);

                constraintLists.Add(constraintList);
            }

            return constraintLists.AsReadOnly();
        }

        private static IReadOnlyCollection<string> DetermineGenericTypeParameters(TypeDeclarationSyntax node)
        {
            var typeParameters = new List<string>();

            if (node.TypeParameterList == null)
            {
                return typeParameters;
            }

            foreach (var typeParameter in node.TypeParameterList.Parameters)
            {
                typeParameters.Add(typeParameter.Identifier.Text);
            }

            return typeParameters;
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

        private IReadOnlyCollection<IClassDefinition> DetermineChildClasses(SyntaxNode node)
        {
            var childNodes = node.ChildNodes().OfType<ClassDeclarationSyntax>();
            var childTypes = childNodes.Select(childNode => new ClassDefinition(this, childNode)).ToList();

            return childTypes.AsReadOnly();
        }

        private IReadOnlyCollection<IInterfaceDefinition> DetermineChildInterfaces(SyntaxNode node)
        {
            var childNodes = node.ChildNodes().OfType<InterfaceDeclarationSyntax>();
            var childTypes = childNodes.Select(childNode => new InterfaceDefinition(this, childNode)).ToList();

            return childTypes.AsReadOnly();
        }

        private IReadOnlyCollection<IPropertyDefinition> DetermineProperties(SyntaxNode node)
        {
            var childNodes = node.ChildNodes().OfType<PropertyDeclarationSyntax>();
            var childTypes = childNodes.Select(childNode => new PropertyDefinition(this, childNode)).ToList();

            return childTypes.AsReadOnly();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IClassDefinition> ChildClasses { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IInterfaceDefinition> ChildInterfaces { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<ITypeDefinition> ChildTypes { get; }

        /// <inheritdoc />
        public ITypeDefinition? DeclaringType { get; }

        /// <inheritdoc />
        public abstract string Description { get; }

        /// <inheritdoc />
        public string FullName { get; }

        /// <inheritdoc />
        public string FullRawName { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IConstraintListDefinition> GenericConstraints { get; protected set; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> GenericTypeParameters { get; }

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
        public IReadOnlyCollection<IPropertyDefinition> Properties { get; }

        /// <inheritdoc />
        public string RawName { get; }

        /// <inheritdoc />
        public string Scope { get; }
    }
}