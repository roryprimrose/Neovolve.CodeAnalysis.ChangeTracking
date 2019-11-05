namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Linq;
    using EnsureThat;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class FieldResolver : MemberResolver, INodeResolver
    {
        public bool IsSupported(SyntaxNode node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            return node is FieldDeclarationSyntax;
        }

        public NodeDefinition Resolve(SyntaxNode node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));
            Ensure.Type.IsOfType(node, typeof(FieldDeclarationSyntax), nameof(node));

            var syntaxNode = (FieldDeclarationSyntax) node;

            var property = Resolve<NodeDefinition>(syntaxNode);

            property.Name = syntaxNode.Declaration.Variables.Single().Identifier.Text;
            property.ReturnType = syntaxNode.Declaration.Type.ToString();

            return property;
        }

        public bool EvaluateChildren { get; } = false;

        public bool SkipNode { get; } = false;
    }
}