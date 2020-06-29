namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public interface INodeScanner
    {
        IEnumerable<OldMemberDefinition> FindDefinitions(IEnumerable<SyntaxNode> nodes);
    }
}