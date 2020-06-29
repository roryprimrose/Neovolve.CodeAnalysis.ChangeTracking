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

        public OldMemberDefinition Resolve(SyntaxNode node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));
            Ensure.Type.IsOfType(node, typeof(FieldDeclarationSyntax), nameof(node));

            var syntaxNode = (FieldDeclarationSyntax) node;

            var member = Resolve<OldMemberDefinition>(syntaxNode);

            member.MemberType = MemberType.Field;
            member.Name = syntaxNode.Declaration.Variables.Single().Identifier.Text;
            member.ReturnType = syntaxNode.Declaration.Type.ToString();

            return member;
        }

        public bool EvaluateChildren { get; } = false;

        public bool SkipNode { get; } = false;
    }
}