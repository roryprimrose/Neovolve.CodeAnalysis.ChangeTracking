﻿namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="StructDefinition" />
    ///     class is used to describe a class.
    /// </summary>
    public class StructDefinition : TypeDefinition, IStructDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StructDefinition" /> class.
        /// </summary>
        /// <param name="node">The syntax node that defines the class.</param>
        public StructDefinition(StructDeclarationSyntax node) : base(node)
        {
            Fields = DetermineFields(node);
            Constructors = DetermineConstructors(node);
            Modifiers = DetermineModifiers(node);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StructDefinition" /> class.
        /// </summary>
        /// <param name="declaringType">The parent type that declares the class.</param>
        /// <param name="node">The syntax node that defines the class.</param>
        public StructDefinition(ITypeDefinition declaringType, StructDeclarationSyntax node) : base(declaringType, node)
        {
            Fields = DetermineFields(node);
            Constructors = DetermineConstructors(node);
            Modifiers = DetermineModifiers(node);
        }

        /// <inheritdoc />
        public override void MergePartialType(ITypeDefinition partialType)
        {
            base.MergePartialType(partialType);

            var partialStructType = (IStructDefinition)partialType;

            Constructors = MergeMembers(Constructors, partialStructType.Constructors);
            Fields = MergeMembers(Fields, partialStructType.Fields);
            Modifiers = Modifiers | partialStructType.Modifiers;
        }

        private static StructModifiers DetermineModifiers(StructDeclarationSyntax node)
        {
            var isPartial = node.Modifiers.HasModifier(SyntaxKind.PartialKeyword);

            if (node.Modifiers.HasModifier(SyntaxKind.ReadOnlyKeyword))
            {
                if (isPartial)
                {
                    return StructModifiers.ReadOnlyPartial;
                }

                return StructModifiers.ReadOnly;
            }

            if (isPartial)
            {
                return StructModifiers.Partial;
            }

            return StructModifiers.None;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IConstructorDefinition> Constructors { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IFieldDefinition> Fields { get; private set; }

        /// <inheritdoc />
        public StructModifiers Modifiers { get; private set; }
    }
}