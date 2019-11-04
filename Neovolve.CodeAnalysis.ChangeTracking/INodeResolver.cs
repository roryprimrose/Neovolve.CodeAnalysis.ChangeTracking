namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Microsoft.CodeAnalysis;

    public interface INodeResolver
    {
        bool IsSupported(SyntaxNode node);

        NodeDefinition Resolve(SyntaxNode node);

        bool EvaluateChildren { get; }
    }
}