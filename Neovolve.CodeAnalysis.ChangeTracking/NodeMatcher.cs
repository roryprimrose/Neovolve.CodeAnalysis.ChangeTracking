namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using EnsureThat;

    public class NodeMatcher : INodeMatcher
    {
        public virtual NodeMatch GetMatch(NodeDefinition oldNode, NodeDefinition newNode)
        {
            Ensure.Any.IsNotNull(oldNode, nameof(oldNode));
            Ensure.Any.IsNotNull(newNode, nameof(newNode));

            if (!string.Equals(oldNode.Namespace, newNode.Namespace, StringComparison.Ordinal))
            {
                return null;
            }

            if (!string.Equals(oldNode.OwningType, newNode.OwningType, StringComparison.Ordinal))
            {
                return null;
            }

            if (!string.Equals(oldNode.Name, newNode.Name, StringComparison.Ordinal))
            {
                return null;
            }

            return new NodeMatch(oldNode, newNode);
        }

        public virtual bool IsSupported(NodeDefinition node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var nodeType = node.GetType();

            if (nodeType == typeof(PropertyDefinition))
            {
                return true;
            }

            // We don't want to support any other derived types here
            return nodeType == typeof(NodeDefinition);
        }
    }
}