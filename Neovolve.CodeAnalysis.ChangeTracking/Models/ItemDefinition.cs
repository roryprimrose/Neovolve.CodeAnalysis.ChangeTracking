namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using Microsoft.CodeAnalysis;
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

            Location = DetermineLocation(node);
        }
        
        private static DefinitionLocation DetermineLocation(CSharpSyntaxNode node)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            string filePath = string.Empty;
            var location = node.GetLocation();

            if (location.IsInSource
                && location.Kind == LocationKind.SourceFile)
            {
                filePath = location.SourceTree?.FilePath ?? string.Empty;
            }

            var startPosition = location.GetLineSpan().StartLinePosition;

            var lineIndex = startPosition.Line;
            var characterIndex = startPosition.Character;

            return new DefinitionLocation(filePath, lineIndex, characterIndex);
        }

        /// <inheritdoc />
        public DefinitionLocation Location { get; }

        /// <inheritdoc />
        public abstract string Name { get; }
    }
}