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

        private static FieldModifiers DetermineModifiers(FieldDeclarationSyntax node)
        {
            var isStatic = node.Modifiers.HasModifier(SyntaxKind.StaticKeyword);
            var isReadOnly = node.Modifiers.HasModifier(SyntaxKind.ReadOnlyKeyword);

            if (isStatic)
            {
                if (isReadOnly)
                {
                    return FieldModifiers.StaticReadOnly;
                }

                return FieldModifiers.Static;
            }

            if (isReadOnly)
            {
                return FieldModifiers.ReadOnly;
            }

            return FieldModifiers.None;
        }

        /// <inheritdoc />
        public override string Description => $"Field {FullName}";

        /// <inheritdoc />
        public override string FullName { get; }

        /// <inheritdoc />
        public override string FullRawName { get; }

        /// <inheritdoc />
        public FieldModifiers Modifiers { get; }

        /// <inheritdoc />
        public override string Name { get; }

        /// <inheritdoc />
        public override string RawName { get; }

        /// <inheritdoc />
        public override string ReturnType { get; }
    }
}