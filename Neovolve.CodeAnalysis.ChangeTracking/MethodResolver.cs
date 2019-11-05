namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class MethodResolver : INodeResolver
    {
        public bool IsSupported(SyntaxNode node)
        {
            return node is MemberDeclarationSyntax;
        }

        public NodeDefinition Resolve(SyntaxNode node)
        {
            throw new NotSupportedException();
        }

        public bool EvaluateChildren { get; } = false;

        public bool SkipNode { get; } = true;
    }
}