namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="InterfaceDefinition" />
    ///     class is used to describe a interface.
    /// </summary>
    public class InterfaceDefinition : TypeDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InterfaceDefinition" /> class.
        /// </summary>
        /// <param name="node">The syntax node that defines the class.</param>
        public InterfaceDefinition(InterfaceDeclarationSyntax node) : base(node)
        {
            GenericConstraints = DetermineGenericConstraints(node);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InterfaceDefinition" /> class.
        /// </summary>
        /// <param name="declaringType">The parent type that declares the class.</param>
        /// <param name="node">The syntax node that defines the class.</param>
        public InterfaceDefinition(TypeDefinition declaringType, InterfaceDeclarationSyntax node) : base(declaringType,
            node)
        {
            GenericConstraints = DetermineGenericConstraints(node);
        }

        private static IReadOnlyCollection<ConstraintListDefinition> DetermineGenericConstraints(
            InterfaceDeclarationSyntax node)
        {
            var constraintLists = new List<ConstraintListDefinition>();

            foreach (var clauses in node.ConstraintClauses)
            {
                var constraintList = new ConstraintListDefinition(clauses);

                constraintLists.Add(constraintList);
            }

            return constraintLists.AsReadOnly();
        }
    }
}