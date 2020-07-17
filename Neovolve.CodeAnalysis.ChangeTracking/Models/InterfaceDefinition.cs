namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
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
        }
    }
}