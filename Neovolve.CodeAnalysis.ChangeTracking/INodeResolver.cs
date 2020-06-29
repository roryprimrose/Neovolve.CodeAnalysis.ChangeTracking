namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Microsoft.CodeAnalysis;

    public interface INodeResolver
    {
        bool IsSupported(SyntaxNode node);

        OldMemberDefinition Resolve(SyntaxNode node);

        bool EvaluateChildren { get; }

        bool SkipNode { get; }
    }
}