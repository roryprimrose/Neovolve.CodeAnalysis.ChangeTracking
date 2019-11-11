namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Microsoft.CodeAnalysis;

    public interface INodeResolver
    {
        bool IsSupported(SyntaxNode node);

        MemberDefinition Resolve(SyntaxNode node);

        bool EvaluateChildren { get; }

        bool SkipNode { get; }
    }
}