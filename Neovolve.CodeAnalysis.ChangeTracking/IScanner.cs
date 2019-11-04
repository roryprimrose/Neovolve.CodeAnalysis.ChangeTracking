namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public interface IScanner
    {
        IEnumerable<NodeDefinition> FindDefinitions(IEnumerable<SyntaxNode> nodes);
    }
}