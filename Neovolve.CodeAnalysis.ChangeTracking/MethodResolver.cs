namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using EnsureThat;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class MethodResolver : INodeResolver
    {
        public bool IsSupported(SyntaxNode node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            return node is MethodDeclarationSyntax;
        }

        public OldMemberDefinition Resolve(SyntaxNode node)
        {
            throw new NotSupportedException();
        }

        public bool EvaluateChildren { get; } = false;

        public bool SkipNode { get; } = true;
    }
}