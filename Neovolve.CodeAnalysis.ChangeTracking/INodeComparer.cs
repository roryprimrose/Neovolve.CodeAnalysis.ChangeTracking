namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public interface INodeComparer
    {
        ChangeType Compare(NodeDefinition oldNode, NodeDefinition newNode);

        bool IsSupported(NodeDefinition node);
    }
}