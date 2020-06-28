namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Reflection.Metadata;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class ClassDefinition : TypeDefinition
    {
        public ClassDefinition(ClassDeclarationSyntax node) : base(node)
        {
        }
        
        public ClassDefinition(TypeDefinition parentType, ClassDeclarationSyntax node) : base(parentType, node)
        {
        }
    }
}