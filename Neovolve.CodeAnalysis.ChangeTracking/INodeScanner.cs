namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public interface INodeScanner
    {
        IEnumerable<MemberDefinition> FindDefinitions(IEnumerable<SyntaxNode> nodes);
    }
}