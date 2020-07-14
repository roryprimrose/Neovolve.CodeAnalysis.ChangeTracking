namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;

    public abstract class ItemDefinition : IItemDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ElementDefinition" /> class.
        /// </summary>
        /// <param name="node">The syntax node that defines the type.</param>
        protected ItemDefinition(CSharpSyntaxNode node)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            Location = node.DetermineLocation();
        }

        /// <inheritdoc />
        public abstract string Description { get; }

        /// <inheritdoc />
        public DefinitionLocation Location { get; }

        /// <inheritdoc />
        public abstract string Name { get; }
    }
}