namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="FieldDefinition" />
    ///     class describes a field on a type.
    /// </summary>
    public class FieldDefinition : MemberDefinition, IFieldDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FieldDefinition" /> class.
        /// </summary>
        /// <param name="declaringType">The type that declares the field.</param>
        /// <param name="node">The node that defines the argument.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="node" /> parameter is <c>null</c>.</exception>
        public FieldDefinition(ITypeDefinition declaringType, FieldDeclarationSyntax node) : base(node, declaringType)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            var name = node.Declaration.Variables.Single().Identifier.Text;

            Modifiers = DetermineModifiers(node);
            ReturnType = node.Declaration.Type.ToString();
            Name = name;
            RawName = name;
            FullName = DeclaringType.FullName + "." + name;
            FullRawName = DeclaringType.FullRawName + "." + name;
        }

        private static MemberModifiers DetermineModifiers(FieldDeclarationSyntax node)
        {
            // TODO: Make this an extension method that is shared with PropertyDefinition
            var isVirtual = node.Modifiers.HasModifier(SyntaxKind.VirtualKeyword);
            var isAbstract = node.Modifiers.HasModifier(SyntaxKind.AbstractKeyword);
            var isNew = node.Modifiers.HasModifier(SyntaxKind.NewKeyword);
            var isOverride = node.Modifiers.HasModifier(SyntaxKind.OverrideKeyword);
            var isStatic = node.Modifiers.HasModifier(SyntaxKind.StaticKeyword);
            var isSealed = node.Modifiers.HasModifier(SyntaxKind.SealedKeyword);

            if (isNew)
            {
                if (isAbstract)
                {
                    if (isVirtual)
                    {
                        return MemberModifiers.NewAbstractVirtual;
                    }

                    return MemberModifiers.NewAbstract;
                }

                if (isStatic)
                {
                    return MemberModifiers.NewStatic;
                }

                if (isVirtual)
                {
                    return MemberModifiers.NewVirtual;
                }

                return MemberModifiers.New;
            }

            if (isAbstract)
            {
                if (isOverride)
                {
                    return MemberModifiers.AbstractOverride;
                }

                return MemberModifiers.Abstract;
            }

            if (isOverride)
            {
                if (isSealed)
                {
                    return MemberModifiers.SealedOverride;
                }

                return MemberModifiers.Override;
            }

            if (isSealed)
            {
                return MemberModifiers.Sealed;
            }

            if (isStatic)
            {
                return MemberModifiers.Static;
            }

            if (isVirtual)
            {
                return MemberModifiers.Virtual;
            }

            return MemberModifiers.None;
        }

        /// <inheritdoc />
        public override string Description => $"Field {FullName}";

        /// <inheritdoc />
        public override string FullName { get; }

        /// <inheritdoc />
        public override string FullRawName { get; }

        /// <inheritdoc />
        public MemberModifiers Modifiers { get; }

        /// <inheritdoc />
        public override string Name { get; }

        /// <inheritdoc />
        public override string RawName { get; }

        /// <inheritdoc />
        public override string ReturnType { get; }
    }
}