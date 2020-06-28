namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class InterfaceDefinition : TypeDefinition
    {
        public InterfaceDefinition(InterfaceDeclarationSyntax node) : base(node)
        {
        }
        
        public InterfaceDefinition(TypeDefinition parentType, InterfaceDeclarationSyntax node) : base(parentType, node)
        {
        }
    }
}