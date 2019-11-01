namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Microsoft.CodeAnalysis;

    public interface INodeResolver<TN, out TD> where TN : SyntaxNode
    {
        TD Resolve(SyntaxNode node);

        bool EvaluateChildren { get; }
    }
}