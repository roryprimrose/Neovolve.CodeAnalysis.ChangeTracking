﻿namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="FieldDefinition" />
    ///     class describes a field on a type.
    /// </summary>
    public class FieldDefinition : IFieldDefinition
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
            Scope = node.DetermineScope();
            Name = node.Declaration.Variables.Single().Identifier.Text;
            RawName = Name;
            FullRawName = declaringType.FullRawName + "." + RawName;
            FullName = declaringType.FullName + "." + Name;

            IsVisible = node.IsVisible();
            ReturnType = node.Declaration.Type.ToString();
            Attributes = node.DetermineAttributes(this);
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IAttributeDefinition> Attributes { get; }

        /// <inheritdoc />
        public ITypeDefinition DeclaringType { get; }

        /// <inheritdoc />
        public string Description => $"Field {FullName}";

        /// <inheritdoc />
        public string FullName { get; }

        /// <inheritdoc />
        public string FullRawName { get; }

        /// <inheritdoc />
        public bool IsVisible { get; }

        /// <inheritdoc />
        public DefinitionLocation Location { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string RawName { get; }

        /// <inheritdoc />
        public string ReturnType { get; }

        /// <inheritdoc />
        public string Scope { get; }
    }
}