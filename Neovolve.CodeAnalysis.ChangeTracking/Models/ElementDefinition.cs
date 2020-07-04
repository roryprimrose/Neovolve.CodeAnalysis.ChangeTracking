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

            IsVisible = node.IsVisible();
            Attributes = node.DetermineAttributes(this);
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; }

        /// <inheritdoc />
        public abstract string FullName { get; }

        /// <inheritdoc />
        public abstract string FullRawName { get; }

        /// <inheritdoc />
        public bool IsVisible { get; }

        /// <inheritdoc />
        public abstract string RawName { get; }
    }
}