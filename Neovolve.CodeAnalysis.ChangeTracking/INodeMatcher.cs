namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public interface INodeMatcher
    {
        NodeMatch GetMatch(NodeDefinition oldDefinition, NodeDefinition newDefinition);
    }
}