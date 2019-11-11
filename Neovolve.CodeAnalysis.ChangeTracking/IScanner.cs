namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public interface IScanner
    {
        IEnumerable<MemberDefinition> FindDefinitions(IEnumerable<SyntaxNode> nodes);
    }
}