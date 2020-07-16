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
        /// <param name="ordinalIndex">The ordinal index where the argument exists in the list of arguments.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="node" /> parameter is <c>null</c>.</exception>
        public ArgumentDefinition(AttributeArgumentSyntax node, int? ordinalIndex)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            Location = node.DetermineLocation();
            Value = node.Expression.ToString();

            if (node.NameColon == null)
            {
                OrdinalIndex = ordinalIndex;
                ParameterName = string.Empty;
                Name = Value;
                ArgumentType = ArgumentType.Ordinal;
                Description = $"Ordinal argument {Value}";
            }
            else
            {
                OrdinalIndex = null;
                ParameterName = node.NameColon.Name.ToString();
                Name = ParameterName;
                ArgumentType = ArgumentType.Named;
                Description = $"Named argument {Name}";
            }
        }

        /// <inheritdoc />
        public ArgumentType ArgumentType { get; }

        /// <inheritdoc />
        public string Description { get; }

        /// <inheritdoc />
        public DefinitionLocation Location { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public int? OrdinalIndex { get; }

        /// <inheritdoc />
        public string ParameterName { get; }

        /// <inheritdoc />
        public string Value { get; }
    }
}