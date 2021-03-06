﻿namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="ArgumentDefinition" />
    ///     class describes an argument for an attribute.
    /// </summary>
    public class ArgumentDefinition : ItemDefinition, IArgumentDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ArgumentDefinition" /> class.
        /// </summary>
        /// <param name="node">The node that defines the argument.</param>
        /// <param name="ordinalIndex">The ordinal index where the argument exists in the list of arguments.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="node" /> parameter is <c>null</c>.</exception>
        public ArgumentDefinition(AttributeArgumentSyntax node, int? ordinalIndex) : base(node)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            Declaration = node.ToFullString();
            Value = node.Expression.ToString();

            if (node.NameColon == null)
            {
                OrdinalIndex = ordinalIndex;
                ParameterName = string.Empty;
                Name = Value;
                ArgumentType = ArgumentType.Ordinal;
            }
            else
            {
                OrdinalIndex = null;
                ParameterName = node.NameColon.Name.ToString();
                Name = ParameterName;
                ArgumentType = ArgumentType.Named;
            }
        }

        /// <inheritdoc />
        public ArgumentType ArgumentType { get; }

        /// <inheritdoc />
        public string Declaration { get; }

        /// <inheritdoc />
        public override string Name { get; }

        /// <inheritdoc />
        public int? OrdinalIndex { get; }

        /// <inheritdoc />
        public string ParameterName { get; }

        /// <inheritdoc />
        public string Value { get; }
    }
}