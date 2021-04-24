namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;
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
            Modifiers = DetermineModifiers(node);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClassDefinition" /> class.
        /// </summary>
        /// <param name="declaringType">The parent type that declares the class.</param>
        /// <param name="node">The syntax node that defines the class.</param>
        public ClassDefinition(ITypeDefinition declaringType, ClassDeclarationSyntax node) : base(declaringType, node)
        {
            Fields = DetermineFields(node);
            Modifiers = DetermineModifiers(node);
        }

        /// <inheritdoc />
        public override void MergePartialType(ITypeDefinition partialType)
        {
            base.MergePartialType(partialType);

            var partialClassType = (IClassDefinition) partialType;

            Fields = MergeMembers(Fields, partialClassType.Fields);
            Modifiers = Modifiers | partialClassType.Modifiers;
        }

        private static ClassModifiers DetermineModifiers(ClassDeclarationSyntax node)
        {
            var isPartial = node.Modifiers.HasModifier(SyntaxKind.PartialKeyword);

            if (node.Modifiers.HasModifier(SyntaxKind.SealedKeyword))
            {
                if (isPartial)
                {
                    return ClassModifiers.SealedPartial;
                }

                return ClassModifiers.Sealed;
            }

            if (node.Modifiers.HasModifier(SyntaxKind.AbstractKeyword))
            {
                if (isPartial)
                {
                    return ClassModifiers.AbstractPartial;
                }

                return ClassModifiers.Abstract;
            }

            if (node.Modifiers.HasModifier(SyntaxKind.StaticKeyword))
            {
                if (isPartial)
                {
                    return ClassModifiers.StaticPartial;
                }

                return ClassModifiers.Static;
            }

            if (isPartial)
            {
                return ClassModifiers.Partial;
            }

            return ClassModifiers.None;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IFieldDefinition> Fields { get; private set; }

        /// <inheritdoc />
        public ClassModifiers Modifiers { get; private set; }
    }
}