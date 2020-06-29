namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="ConstraintListDefinition" />
    ///     class identifies a set of generic type constraints.
    /// </summary>
    public class ConstraintListDefinition
    {
        public ConstraintListDefinition(TypeParameterConstraintClauseSyntax node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            Name = node.Name.ToString();
            Constraints = DetermineConstraints(node);
        }

        private static IReadOnlyCollection<string> DetermineConstraints(TypeParameterConstraintClauseSyntax node)
        {
            var constraints = new List<string>();

            foreach (var constraint in node.Constraints)
            {
                constraints.Add(constraint.ToString());
            }

            return constraints.AsReadOnly();
        }

        /// <summary>
        ///     Gets the constraints declared for the generic type definition.
        /// </summary>
        public IReadOnlyCollection<string> Constraints { get; }

        /// <summary>
        ///     Gets the name of the generic type definition.
        /// </summary>
        public string Name { get; }
    }
}