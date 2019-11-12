namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public interface IChangeCalculator
    {
        ChangeType CalculateChange(IEnumerable<SyntaxNode> oldNodes, IEnumerable<SyntaxNode> newNodes);
    }
}