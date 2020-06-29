namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class InterfaceDefinition : TypeDefinition
    {
        public InterfaceDefinition(InterfaceDeclarationSyntax node) : base(node)
        {
            GenericConstraints = DetermineGenericConstraints(node);
        }

        public InterfaceDefinition(TypeDefinition parentType, InterfaceDeclarationSyntax node) : base(parentType, node)
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