﻿namespace Neovolve.CodeAnalysis.ChangeTracking.Models
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
            Modifiers = DetermineModifiers(node);
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

        private IReadOnlyCollection<FieldDefinition> DetermineFields(SyntaxNode node)
        {
            var childNodes = node.ChildNodes().OfType<FieldDeclarationSyntax>();
            var childTypes = childNodes.Select(childNode => new FieldDefinition(this, childNode)).FastToList();

            return childTypes.AsReadOnly();
        }

        /// <inheritdoc />
        public override string Description => $"Class {FullName}";

        /// <inheritdoc />
        public IReadOnlyCollection<IFieldDefinition> Fields { get; }

        /// <inheritdoc />
        public ClassModifiers Modifiers { get; }
    }
}