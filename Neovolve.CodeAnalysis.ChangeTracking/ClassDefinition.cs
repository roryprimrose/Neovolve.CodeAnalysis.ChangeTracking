namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class ClassDefinition : TypeDefinition
    {
        public ClassDefinition(ClassDeclarationSyntax node) : base(node)
        {
            GenericConstraints = DetermineGenericConstraints(node);
        }

        public ClassDefinition(TypeDefinition parentType, ClassDeclarationSyntax node) : base(parentType, node)
        {
            GenericConstraints = DetermineGenericConstraints(node);
        }

        private static IReadOnlyCollection<ConstraintListDefinition> DetermineGenericConstraints(
            ClassDeclarationSyntax node)
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