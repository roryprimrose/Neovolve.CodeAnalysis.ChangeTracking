namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public interface IChangeCalculator
    {
        SemVerChangeType CalculateChange(IEnumerable<SyntaxNode> oldNodes, IEnumerable<SyntaxNode> newNodes);
    }
}