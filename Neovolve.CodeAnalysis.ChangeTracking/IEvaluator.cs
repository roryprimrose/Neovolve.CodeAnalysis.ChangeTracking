namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public interface IEvaluator
    {
        ChangeType CompareNodes(IEnumerable<SyntaxNode> oldNodes, IEnumerable<SyntaxNode> newNodes);
    }
}