namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public abstract class ElementDefinition : ItemDefinition, IElementDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ElementDefinition" /> class.
        /// </summary>
        /// <param name="node">The syntax node that defines the type.</param>
        protected ElementDefinition(MemberDeclarationSyntax node) : base(node)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            DeclaredModifiers = node.Modifiers.ToString();
            Attributes = DetermineAttributes(node, this);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ElementDefinition" /> class.
        /// </summary>
        /// <param name="node">The syntax node that defines the type.</param>
        protected ElementDefinition(ParameterSyntax node) : base(node)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            DeclaredModifiers = node.Modifiers.ToString();
            Attributes = DetermineAttributes(node, this);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ElementDefinition" /> class.
        /// </summary>
        /// <param name="node">The syntax node that defines the type.</param>
        protected ElementDefinition(AccessorDeclarationSyntax node) : base(node)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            DeclaredModifiers = node.Modifiers.ToFullString();
            Attributes = DetermineAttributes(node, this);
        }

        public static IReadOnlyCollection<IAttributeDefinition> DetermineAttributes(AccessorDeclarationSyntax node,
            IElementDefinition declaringItem)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));
            declaringItem = declaringItem ?? throw new ArgumentNullException(nameof(declaringItem));

            var attributeList = node.AttributeLists;

            return DetermineAttributes(declaringItem, attributeList);
        }

        private static IReadOnlyCollection<IAttributeDefinition> DetermineAttributes(IElementDefinition declaringItem,
            SyntaxList<AttributeListSyntax> attributeList)
        {
            var definitions = new List<IAttributeDefinition>();

            foreach (var list in attributeList)
            {
                foreach (var attribute in list.Attributes)
                {
                    var definition = new AttributeDefinition(attribute);

                    definitions.Add(definition);
                }
            }

            return definitions.AsReadOnly();
        }

        private static IReadOnlyCollection<IAttributeDefinition> DetermineAttributes(ParameterSyntax node,
            IElementDefinition declaringItem)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));
            declaringItem = declaringItem ?? throw new ArgumentNullException(nameof(declaringItem));

            var attributeList = node.AttributeLists;

            return DetermineAttributes(declaringItem, attributeList);
        }

        private static IReadOnlyCollection<IAttributeDefinition> DetermineAttributes(MemberDeclarationSyntax node,
            IElementDefinition declaringItem)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));
            declaringItem = declaringItem ?? throw new ArgumentNullException(nameof(declaringItem));

            var attributeList = node.AttributeLists;

            return DetermineAttributes(declaringItem, attributeList);
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; protected set; }

        /// <inheritdoc />
        public string DeclaredModifiers { get; protected set; }

        /// <inheritdoc />
        public abstract string FullName { get; }

        /// <inheritdoc />
        public abstract string FullRawName { get; }

        /// <inheritdoc />
        public bool IsVisible { get; protected set; }

        /// <inheritdoc />
        public abstract string RawName { get; }
    }
}