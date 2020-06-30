namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="TypeDefinition" />
    ///     class is used to describe a class.
    /// </summary>
    public class ClassDefinition : TypeDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ClassDefinition" /> class.
        /// </summary>
        /// <param name="node">The syntax node that defines the class.</param>
        public ClassDefinition(ClassDeclarationSyntax node) : base(node)
        {
            GenericConstraints = DetermineGenericConstraints(node);
            Fields = DetermineFields(node);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClassDefinition" /> class.
        /// </summary>
        /// <param name="declaringType">The parent type that declares the class.</param>
        /// <param name="node">The syntax node that defines the class.</param>
        public ClassDefinition(TypeDefinition declaringType, ClassDeclarationSyntax node) : base(declaringType, node)
        {
            GenericConstraints = DetermineGenericConstraints(node);
            Fields = DetermineFields(node);
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

        private IReadOnlyCollection<FieldDefinition> DetermineFields(SyntaxNode node)
        {
            var childNodes = node.ChildNodes().OfType<FieldDeclarationSyntax>();
            var childTypes = childNodes.Select(childNode => new FieldDefinition(this, childNode)).ToList();

            return childTypes.AsReadOnly();
        }

        /// <summary>
        /// Gets the fields declared on the class.
        /// </summary>
        public IReadOnlyCollection<FieldDefinition> Fields { get; }
    }
}