namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public abstract class BaseTypeDefinition<T> : ElementDefinition, IBaseTypeDefinition<T> where T : struct, Enum
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseTypeDefinition{T}" /> class.
        /// </summary>
        /// <typeparam name="T">The type of access modifier this type exposes.</typeparam>
        /// <param name="node">The syntax node that defines the type.</param>
        protected BaseTypeDefinition(BaseTypeDeclarationSyntax node) : base(node)
        {
            DeclaringType = null;

            Namespace = DetermineNamespace(node);
            ImplementedTypes = DetermineImplementedTypes(node);

            var name = DetermineName(node);
            var rawName = node.Identifier.Text;

            Name = name;
            RawName = rawName;
            FullRawName = Namespace + "." + rawName;
            FullName = Namespace + "." + name;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseTypeDefinition{T}" /> class.
        /// </summary>
        /// <typeparam name="T">The type of access modifier this type exposes.</typeparam>
        /// <param name="declaringType">The parent type that declares this type.</param>
        /// <param name="node">The syntax node that defines the type.</param>
        protected BaseTypeDefinition(ITypeDefinition declaringType, BaseTypeDeclarationSyntax node) : base(node)
        {
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));

            Namespace = DetermineNamespace(node);
            ImplementedTypes = DetermineImplementedTypes(node);

            var name = DetermineName(node);
            var rawName = node.Identifier.Text;

            Name = name;
            RawName = rawName;
            FullRawName = DeclaringType.FullRawName + "+" + rawName;
            FullName = DeclaringType.FullName + "+" + name;
        }

        protected static string DetermineName(BaseTypeDeclarationSyntax node)
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

        /// <summary>
        ///     Gets the namespace that contains the node.
        /// </summary>
        /// <param name="node">The node to evaluate.</param>
        /// <returns>The namespace that contains the node or <see cref="string.Empty" /> if no namespace is found.</returns>
        protected static string DetermineNamespace(SyntaxNode node)
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

        private static IReadOnlyCollection<string> DetermineImplementedTypes(BaseTypeDeclarationSyntax node)
        {
            var baseList = node.ChildNodes().OfType<BaseListSyntax>().FirstOrDefault();

            if (baseList == null)
            {
                return Array.Empty<string>();
            }

            var childTypes = baseList.Types.Select(x => x.ToString()).FastToList();

            return childTypes.AsReadOnly();
        }

        /// <inheritdoc />
        public T AccessModifiers { get; protected set; }

        /// <inheritdoc />
        public ITypeDefinition? DeclaringType { get; set; }

        /// <inheritdoc />
        public override string FullName { get; }

        /// <inheritdoc />
        public override string FullRawName { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> ImplementedTypes { get; }

        /// <inheritdoc />
        public override string Name { get; }

        /// <inheritdoc />
        public string Namespace { get; set; }

        /// <inheritdoc />
        public override string RawName { get; }
    }
}