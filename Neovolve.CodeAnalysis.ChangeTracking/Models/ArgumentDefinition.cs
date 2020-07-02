namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="ArgumentDefinition" />
    ///     class describes an argument for an attribute.
    /// </summary>
    public class ArgumentDefinition : IArgumentDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ArgumentDefinition" /> class.
        /// </summary>
        /// <param name="node">The node that defines the argument.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="node" /> parameter is <c>null</c>.</exception>
        public ArgumentDefinition(AttributeArgumentSyntax node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            Location = node.DetermineLocation();
            Value = node.Expression.ToString();

            if (node.NameColon == null)
            {
                Name = string.Empty;
                ArgumentType = ArgumentType.Ordinal;
                Description = $"Ordinal argument {Value}";
            }
            else
            {
                Name = node.NameColon.Name.ToString();
                ArgumentType = ArgumentType.Named;
                Description = $"Named argument {Name}";
            }
        }

        /// <inheritdoc />
        public ArgumentType ArgumentType { get; }

        /// <inheritdoc />
        public DefinitionLocation Location { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Value { get; }

        /// <inheritdoc />
        public string Description { get; }
    }
}