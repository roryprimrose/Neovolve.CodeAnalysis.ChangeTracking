namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public interface IMatchEvaluator
    {
        MatchResults CompareNodes(IEnumerable<SyntaxNode> oldNodes, IEnumerable<SyntaxNode> newNodes);
    }
}