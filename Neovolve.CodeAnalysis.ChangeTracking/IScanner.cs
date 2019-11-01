namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;

    public interface IScanner
    {
        Task<IAsyncEnumerable<NodeDefinition>> FindDefinitions(IAsyncEnumerable<SyntaxNode> nodes);
    }
}