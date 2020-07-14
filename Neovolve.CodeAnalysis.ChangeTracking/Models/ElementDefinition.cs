namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using System.Collections.Generic;
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
            Attributes = node.DetermineAttributes(this);
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

            var definitions = new List<IAttributeDefinition>();

            foreach (var list in node.AttributeLists)
            {
                foreach (var attribute in list.Attributes)
                {
                    var definition = new AttributeDefinition(declaringItem, attribute);

                    definitions.Add(definition);
                }
            }

            return definitions.AsReadOnly();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; }

        /// <inheritdoc />
        public string DeclaredModifiers { get; }

        /// <inheritdoc />
        public abstract string FullName { get; }

        /// <inheritdoc />
        public abstract string FullRawName { get; }

        /// <inheritdoc />
        public abstract bool IsVisible { get; }

        /// <inheritdoc />
        public abstract string RawName { get; }
    }
}