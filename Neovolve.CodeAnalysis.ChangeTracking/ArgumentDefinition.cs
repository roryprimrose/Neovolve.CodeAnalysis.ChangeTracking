namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="ArgumentDefinition" />
    ///     class describes an argument for an attribute.
    /// </summary>
    public class ArgumentDefinition
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

            Value = node.Expression.ToString();

            if (node.NameColon == null)
            {
                Name = string.Empty;
                ArgumentType = ArgumentType.Ordinal;
            }
            else
            {
                Name = node.NameColon.Name.ToString();
                ArgumentType = ArgumentType.Named;
            }
        }

        /// <summary>
        ///     Gets the type of argument.
        /// </summary>
        public ArgumentType ArgumentType { get; }

        /// <summary>
        ///     Gets the name of the argument.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the value of the argument.
        /// </summary>
        public string Value { get; }
    }
}