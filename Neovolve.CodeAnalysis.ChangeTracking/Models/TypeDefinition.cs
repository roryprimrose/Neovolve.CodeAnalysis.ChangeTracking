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
    public abstract class TypeDefinition : ElementDefinition, ITypeDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeDefinition" /> class.
        /// </summary>
        /// <param name="node">The syntax node that defines the type.</param>
        protected TypeDefinition(TypeDeclarationSyntax node) : base(node)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            var name = DetermineName(node);
            var rawName = node.Identifier.Text;

            DeclaringType = null;
            Namespace = DetermineNamespace(node);
            AccessModifier = node.DetermineAccessModifier(DeclaringType);
            Name = name;
            RawName = rawName;
            FullRawName = Namespace + "." + rawName;
            FullName = Namespace + "." + name;

            ImplementedTypes = DetermineImplementedTypes(node);
            Properties = DetermineProperties(node);
            ChildClasses = DetermineChildClasses(node);
            ChildInterfaces = DetermineChildInterfaces(node);
            ChildTypes = DetermineChildTypes(ChildClasses, ChildInterfaces);
            GenericTypeParameters = DetermineGenericTypeParameters(node);
            GenericConstraints = DetermineGenericConstraints(node);

            if (DeclaringType != null
                && DeclaringType.IsVisible == false)
            {
                IsVisible = false;
            }
            else
            {
                // Determine visibility based on the access modifier
                IsVisible = AccessModifier.IsVisible();
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeDefinition" /> class.
        /// </summary>
        /// <param name="declaringType">The parent type that declares this type.</param>
        /// <param name="node">The syntax node that defines the type.</param>
        protected TypeDefinition(ITypeDefinition declaringType, TypeDeclarationSyntax node) : base(node)
        {
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));

            var name = DetermineName(node);
            var rawName = node.Identifier.Text;

            Namespace = DetermineNamespace(node);
            AccessModifier = node.DetermineAccessModifier(DeclaringType);
            Name = name;
            RawName = rawName;
            FullRawName = DeclaringType.FullRawName + "+" + rawName;
            FullName = DeclaringType.FullName + "+" + name;

            ImplementedTypes = DetermineImplementedTypes(node);
            Properties = DetermineProperties(node);
            ChildClasses = DetermineChildClasses(node);
            ChildInterfaces = DetermineChildInterfaces(node);
            ChildTypes = DetermineChildTypes(ChildClasses, ChildInterfaces);
            GenericTypeParameters = DetermineGenericTypeParameters(node);
            GenericConstraints = DetermineGenericConstraints(node);

            if (DeclaringType != null
                && DeclaringType.IsVisible == false)
            {
                IsVisible = false;
            }
            else
            {
                // Determine visibility based on the access modifier
                IsVisible = AccessModifier.IsVisible();
            }
        }

        /// <summary>
        ///     Gets the namespace that contains the node.
        /// </summary>
        /// <param name="node">The node to evaluate.</param>
        /// <returns>The namespace that contains the node or <see cref="string.Empty" /> if no namespace is found.</returns>
        public static string DetermineNamespace(SyntaxNode node)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            var containerNamespace = node.FirstAncestorOrSelf<NamespaceDeclarationSyntax>(x => x != node);

            if (containerNamespace != null)
            {
                var parentNamespace = DetermineNamespace(containerNamespace);

                var namespaceValue = containerNamespace.Name.GetText().ToString().Trim();

                if (string.IsNullOrWhiteSpace(parentNamespace))
                {
                    return namespaceValue;
                }

                return parentNamespace + "." + namespaceValue;
            }

            return string.Empty;
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
        public AccessModifier AccessModifier { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IClassDefinition> ChildClasses { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IInterfaceDefinition> ChildInterfaces { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<ITypeDefinition> ChildTypes { get; }

        /// <inheritdoc />
        public ITypeDefinition? DeclaringType { get; }

        /// <inheritdoc />
        public override string FullName { get; }

        /// <inheritdoc />
        public override string FullRawName { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IConstraintListDefinition> GenericConstraints { get; protected set; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> GenericTypeParameters { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> ImplementedTypes { get; }

        /// <inheritdoc />
        public override bool IsVisible { get; }

        /// <inheritdoc />
        public override string Name { get; }

        /// <inheritdoc />
        public string Namespace { get; set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IPropertyDefinition> Properties { get; }

        /// <inheritdoc />
        public override string RawName { get; }
    }
}