namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public interface IChangeCalculator
    {
        ChangeCalculatorResult CalculateChanges(IEnumerable<SyntaxNode> oldNodes, IEnumerable<SyntaxNode> newNodes);
    }
}