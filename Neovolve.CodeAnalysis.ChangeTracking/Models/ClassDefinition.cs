namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="ClassDefinition" />
    ///     class is used to describe a class.
    /// </summary>
    public class ClassDefinition : TypeDefinition, IClassDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ClassDefinition" /> class.
        /// </summary>
        /// <param name="node">The syntax node that defines the class.</param>
        public ClassDefinition(ClassDeclarationSyntax node) : base(node)
        {
            Fields = DetermineFields(node);
            IsAbstract = node.Modifiers.HasModifier(SyntaxKind.AbstractKeyword);
            IsPartial = node.Modifiers.HasModifier(SyntaxKind.PartialKeyword);
            IsSealed = node.Modifiers.HasModifier(SyntaxKind.SealedKeyword);
            IsStatic = node.Modifiers.HasModifier(SyntaxKind.StaticKeyword);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClassDefinition" /> class.
        /// </summary>
        /// <param name="declaringType">The parent type that declares the class.</param>
        /// <param name="node">The syntax node that defines the class.</param>
        public ClassDefinition(ITypeDefinition declaringType, ClassDeclarationSyntax node) : base(declaringType, node)
        {
            Fields = DetermineFields(node);
            IsAbstract = node.Modifiers.HasModifier(SyntaxKind.AbstractKeyword);
            IsPartial = node.Modifiers.HasModifier(SyntaxKind.PartialKeyword);
            IsSealed = node.Modifiers.HasModifier(SyntaxKind.SealedKeyword);
            IsStatic = node.Modifiers.HasModifier(SyntaxKind.StaticKeyword);
        }

        private IReadOnlyCollection<FieldDefinition> DetermineFields(SyntaxNode node)
        {
            var childNodes = node.ChildNodes().OfType<FieldDeclarationSyntax>();
            var childTypes = childNodes.Select(childNode => new FieldDefinition(this, childNode)).ToList();

            return childTypes.AsReadOnly();
        }

        /// <inheritdoc />
        public override string Description => $"Class {FullName}";

        /// <inheritdoc />
        public IReadOnlyCollection<IFieldDefinition> Fields { get; }

        /// <inheritdoc />
        public bool IsAbstract { get; }

        /// <inheritdoc />
        public bool IsPartial { get; }

        /// <inheritdoc />
        public bool IsSealed { get; }

        /// <inheritdoc />
        public bool IsStatic { get; }
    }
}