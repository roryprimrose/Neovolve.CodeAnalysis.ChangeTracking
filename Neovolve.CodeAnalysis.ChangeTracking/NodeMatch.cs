namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using EnsureThat;

    public class NodeMatch
    {
        public NodeMatch(NodeDefinition oldNode, NodeDefinition newNode)
        {
            Ensure.Any.IsNotNull(oldNode, nameof(oldNode));
            Ensure.Any.IsNotNull(newNode, nameof(newNode));

            OldNode = oldNode;
            NewNode = newNode;
        }

        public NodeDefinition NewNode { get; }

        public NodeDefinition OldNode { get; }
    }
}