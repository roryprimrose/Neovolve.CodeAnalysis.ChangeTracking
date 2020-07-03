namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public abstract class ElementDefinition : IElementDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ElementDefinition" /> class.
        /// </summary>
        /// <param name="node">The syntax node that defines the type.</param>
        protected ElementDefinition(MemberDeclarationSyntax node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            Location = node.DetermineLocation();
            AccessModifiers = node.DetermineAccessModifiers();
            Modifiers = DetermineModifiers(node);

            IsVisible = node.IsVisible();
            Attributes = node.DetermineAttributes(this);
        }

        private static string DetermineModifiers(MemberDeclarationSyntax node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var values = new List<string>();

            foreach (var modifier in node.Modifiers)
            {
                switch (modifier.RawKind)
                {
                    case (int) SyntaxKind.PublicKeyword:
                    case (int) SyntaxKind.PrivateKeyword:
                    case (int) SyntaxKind.InternalKeyword:
                    case (int) SyntaxKind.ProtectedKeyword:

                        break;

                    default:

                        values.Add(modifier.Text);

                        break;
                }
            }

            return string.Join(" ", values);
        }

        /// <inheritdoc />
        public string AccessModifiers { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; }

        /// <inheritdoc />
        public abstract string Description { get; }

        /// <inheritdoc />
        public abstract string FullName { get; }

        /// <inheritdoc />
        public abstract string FullRawName { get; }

        /// <inheritdoc />
        public bool IsVisible { get; }

        /// <inheritdoc />
        public DefinitionLocation Location { get; }

        /// <inheritdoc />
        public string Modifiers { get; }

        /// <inheritdoc />
        public abstract string Name { get; }

        /// <inheritdoc />
        public abstract string RawName { get; }
    }
}