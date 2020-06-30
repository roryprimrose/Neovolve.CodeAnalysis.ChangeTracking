namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="FieldDefinition" />
    ///     class describes a field on a type.
    /// </summary>
    public class FieldDefinition : IMemberDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FieldDefinition" /> class.
        /// </summary>
        /// <param name="declaringType">The type that declares the field.</param>
        /// <param name="node">The node that defines the argument.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="node" /> parameter is <c>null</c>.</exception>
        public FieldDefinition(ITypeDefinition declaringType, FieldDeclarationSyntax node)
        {
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            Location = node.DetermineLocation();
            Name = node.Declaration.Variables.Single().Identifier.Text;
            Attributes = node.DetermineAttributes(this);
            FullName = declaringType.FullName + "." + Name;
            IsVisible = node.IsVisible();
            ReturnType = node.Declaration.Type.ToString();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<AttributeDefinition> Attributes { get; }

        /// <inheritdoc />
        public ITypeDefinition? DeclaringType { get; }

        /// <inheritdoc />
        public string FullName { get; }

        /// <inheritdoc />
        public bool IsVisible { get; }

        /// <inheritdoc />
        public DefinitionLocation Location { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        ///     Gets the return type of the member.
        /// </summary>
        public string ReturnType { get; }
    }
}