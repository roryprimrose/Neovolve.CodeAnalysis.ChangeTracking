namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="AttributeDefinition" />
    ///     class is used to define an attribute declaration.
    /// </summary>
    public class AttributeDefinition : ItemDefinition, IAttributeDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AttributeDefinition" /> class.
        /// </summary>
        /// <param name="node">The node that describes the attribute.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="node" /> parameter is <c>null</c>.</exception>
        public AttributeDefinition(AttributeSyntax node) : base(node)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            Name = node.Name.ToString();
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

            var index = 0;

            foreach (var argument in arguments.Arguments)
            {
                var definition = new ArgumentDefinition(argument, index);

                definitions.Add(definition);
                index++;
            }

            return definitions.AsReadOnly();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IArgumentDefinition> Arguments { get; }

        /// <inheritdoc />
        public override string Name { get; }
    }
}