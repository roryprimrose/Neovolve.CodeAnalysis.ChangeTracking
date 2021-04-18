namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="InterfaceDefinition" />
    ///     class is used to describe a interface.
    /// </summary>
    public class InterfaceDefinition : TypeDefinition, IInterfaceDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InterfaceDefinition" /> class.
        /// </summary>
        /// <param name="node">The syntax node that defines the class.</param>
        public InterfaceDefinition(InterfaceDeclarationSyntax node) : base(node)
        {
            Modifiers = DetermineModifiers(node);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InterfaceDefinition" /> class.
        /// </summary>
        /// <param name="declaringType">The parent type that declares the class.</param>
        /// <param name="node">The syntax node that defines the class.</param>
        public InterfaceDefinition(ITypeDefinition declaringType, InterfaceDeclarationSyntax node) : base(
            declaringType,
            node)
        {
            Modifiers = DetermineModifiers(node);
        }

        private static InterfaceModifiers DetermineModifiers(InterfaceDeclarationSyntax node)
        {
            var isPartial = node.Modifiers.HasModifier(SyntaxKind.PartialKeyword);

            if (node.Modifiers.HasModifier(SyntaxKind.NewKeyword))
            {
                if (isPartial)
                {
                    return InterfaceModifiers.NewPartial;
                }

                return InterfaceModifiers.New;
            }

            if (isPartial)
            {
                return InterfaceModifiers.Partial;
            }

            return InterfaceModifiers.None;
        }

        /// <inheritdoc />
        public InterfaceModifiers Modifiers { get; }
    }
}