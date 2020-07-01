namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="AttributeDefinition" />
    ///     class is used to define an attribute declaration.
    /// </summary>
    public class AttributeDefinition : IAttributeDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AttributeDefinition" /> class.
        /// </summary>
        /// <param name="declaringItem">The member that declares the attribute.</param>
        /// <param name="node">The node that describes the attribute.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="declaringItem" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="node" /> parameter is <c>null</c>.</exception>
        public AttributeDefinition(IItemDefinition declaringItem, AttributeSyntax node)
        {
            DeclaredOn = declaringItem ?? throw new ArgumentNullException(nameof(declaringItem));

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            Name = node.Name.ToString();
            Location = node.DetermineLocation();
            Arguments = DetermineParameters(node);
        }

        private static IReadOnlyCollection<IArgumentDefinition> DetermineParameters(AttributeSyntax node)
        {
            var arguments = node.ArgumentList;

            if (arguments == null)
            {
                return Array.Empty<ArgumentDefinition>();
            }

            var definitions = new List<ArgumentDefinition>();

            foreach (var argument in arguments.Arguments)
            {
                var definition = new ArgumentDefinition(argument);

                definitions.Add(definition);
            }

            return definitions.AsReadOnly();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IArgumentDefinition> Arguments { get; }

        /// <inheritdoc />
        public IItemDefinition DeclaredOn { get; }

        /// <inheritdoc />
        public DefinitionLocation Location { get; }

        /// <inheritdoc />
        public string Name { get; }
    }
}