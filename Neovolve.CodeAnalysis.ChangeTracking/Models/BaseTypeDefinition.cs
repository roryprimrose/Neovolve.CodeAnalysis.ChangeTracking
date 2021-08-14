namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public abstract class BaseTypeDefinition : ElementDefinition, IBaseTypeDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseTypeDefinition" /> class.
        /// </summary>
        /// <param name="node">The syntax node that defines the type.</param>
        protected BaseTypeDefinition(BaseTypeDeclarationSyntax node) : base(node)
        {
            DeclaringType = null;

            var name = DetermineName(node);
            var rawName = node.Identifier.Text;

            Namespace = DetermineNamespace(node);
            IsVisible = DetermineIsVisible(node, DeclaringType);
            AccessModifiers = DetermineAccessModifier(node, DeclaringType);

            Name = name;
            RawName = rawName;
            FullRawName = Namespace + "." + rawName;
            FullName = Namespace + "." + name;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseTypeDefinition" /> class.
        /// </summary>
        /// <param name="declaringType">The parent type that declares this type.</param>
        /// <param name="node">The syntax node that defines the type.</param>
        protected BaseTypeDefinition(ITypeDefinition declaringType, BaseTypeDeclarationSyntax node) : base(node)
        {
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));
            
            var name = DetermineName(node);
            var rawName = node.Identifier.Text;

            Namespace = DetermineNamespace(node);
            IsVisible = DetermineIsVisible(node, DeclaringType);
            AccessModifiers = DetermineAccessModifier(node, DeclaringType);

            Name = name;
            RawName = rawName;
            FullRawName = DeclaringType.FullRawName + "+" + rawName;
            FullName = DeclaringType.FullName + "+" + name;
        }

        protected static AccessModifiers DetermineAccessModifier(BaseTypeDeclarationSyntax node,
            ITypeDefinition? declaringType)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            if (declaringType == null)
            {
                return node.Modifiers.DetermineAccessModifier(AccessModifiers.Internal);
            }

            return node.Modifiers.DetermineAccessModifier(AccessModifiers.Private);
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

        private static bool DetermineIsVisible(BaseTypeDeclarationSyntax node, ITypeDefinition? declaringType)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            if (declaringType != null
                && declaringType.IsVisible == false)
            {
                // The parent type is not visible so this one can't be either
                return false;
            }

            // This is either a top level type or the parent type is visible
            // Determine visibility based on the access modifiers
            var accessModifier = DetermineAccessModifier(node, declaringType);

            return accessModifier.IsVisible();
        }

        /// <inheritdoc />
        public AccessModifiers AccessModifiers { get; }

        /// <inheritdoc />
        public ITypeDefinition? DeclaringType { get; set; }

        /// <inheritdoc />
        public override string FullName { get; }

        /// <inheritdoc />
        public override string FullRawName { get; }

        /// <inheritdoc />
        public override bool IsVisible { get; }

        /// <inheritdoc />
        public override string Name { get; }

        /// <inheritdoc />
        public string Namespace { get; set; }

        /// <inheritdoc />
        public override string RawName { get; }
    }
}