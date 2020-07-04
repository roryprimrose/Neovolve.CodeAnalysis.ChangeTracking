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
        public static AccessModifier DetermineAccessModifier(this TypeDeclarationSyntax node,
            ITypeDefinition? declaringType)
        {
            if (declaringType == null)
            {
                return DetermineAccessModifier(node, AccessModifier.Internal);
            }

            return DetermineAccessModifier(node, AccessModifier.Private);
        }

        public static AccessModifier DetermineAccessModifier(this MemberDeclarationSyntax node, ITypeDefinition declaringType)
        {
            declaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));

            // See default values as identified at https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/accessibility-levels
            if (declaringType is IInterfaceDefinition)
            {
                return DetermineAccessModifier(node, AccessModifier.Public);
            }

            if (declaringType is IClassDefinition)
            {
                return DetermineAccessModifier(node, AccessModifier.Private);
            }

            // TODO: Fill these out when the types are supported
            throw new NotSupportedException();
            // Struct default accessmodifier is private
            // Struct default enum is public
        }

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

        public static bool HasModifier(this MemberDeclarationSyntax node, SyntaxKind kind)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            return node.Modifiers.Any(x => x.RawKind == (int) kind);
        }

        private static AccessModifier DetermineAccessModifier(MemberDeclarationSyntax node,
            AccessModifier defaultValue)
        {
            if (node.HasModifier(SyntaxKind.ProtectedKeyword))
            {
                if (node.HasModifier(SyntaxKind.InternalKeyword))
                {
                    return AccessModifier.ProtectedInternal;
                }

                if (node.HasModifier(SyntaxKind.PrivateKeyword))
                {
                    return AccessModifier.ProtectedPrivate;
                }

                return AccessModifier.Protected;
            }

            if (node.HasModifier(SyntaxKind.InternalKeyword))
            {
                return AccessModifier.Internal;
            }

            if (node.HasModifier(SyntaxKind.PrivateKeyword))
            {
                return AccessModifier.Private;
            }

            if (node.HasModifier(SyntaxKind.PublicKeyword))
            {
                return AccessModifier.Public;
            }

            return defaultValue;
        }
    }
}