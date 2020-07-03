namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="NodeExtensions" />
    ///     class provides extension methods for reading node information.
    /// </summary>
    public static class NodeExtensions
    {
        /// <summary>
        ///     Gets the attributes declared on the node.
        /// </summary>
        /// <param name="node">The node to evaluate.</param>
        /// <param name="declaringItem">The item that is declaring the attributes.</param>
        /// <returns>Returns the set of attributes declared on the node.</returns>
        public static IReadOnlyCollection<IAttributeDefinition> DetermineAttributes(this MemberDeclarationSyntax node,
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

        /// <summary>
        ///     Gets the location of the node.
        /// </summary>
        /// <param name="node">The node to evaluate.</param>
        /// <returns>The node location.</returns>
        public static DefinitionLocation DetermineLocation(this CSharpSyntaxNode node)
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

        public static string DetermineAccessModifiers(this MemberDeclarationSyntax node)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            var values = new List<string>();

            foreach (var modifier in node.Modifiers)
            {
                switch (modifier.RawKind)
                {
                    case (int) SyntaxKind.PublicKeyword:
                    case (int) SyntaxKind.PrivateKeyword:
                    case (int) SyntaxKind.InternalKeyword:
                    case (int) SyntaxKind.ProtectedKeyword:

                        values.Add(modifier.Text);

                        break;
                }
            }

            return string.Join(" ", values);
        }

        public static bool HasModifier(this MemberDeclarationSyntax node, SyntaxKind kind)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            return node.Modifiers.Any(x => x.RawKind == (int) kind);
        }
    }
}