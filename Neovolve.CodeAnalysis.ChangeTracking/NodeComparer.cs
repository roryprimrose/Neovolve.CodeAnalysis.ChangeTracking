namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using EnsureThat;

    public class NodeComparer : INodeComparer
    {
        public virtual ChangeType Compare(NodeDefinition oldNode, NodeDefinition newNode)
        {
            Ensure.Any.IsNotNull(oldNode, nameof(oldNode));
            Ensure.Any.IsNotNull(newNode, nameof(newNode));

            // Evaluate all scenarios that could cause a breaking change
            if (oldNode.IsPublic
                && newNode.IsPublic == false)
            {
                return ChangeType.Breaking;
            }

            if (oldNode.ReturnType.Equals(newNode.ReturnType, StringComparison.Ordinal) == false)
            {
                return ChangeType.Breaking;
            }

            // Evaluate all scenarios that could indicate a feature change
            if (oldNode.IsPublic == false
                && newNode.IsPublic)
            {
                return ChangeType.Feature;
            }

            return ChangeType.None;
        }

        public virtual bool IsSupported(NodeDefinition node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            // We don't want to support derived types here
            return node.GetType() == typeof(NodeDefinition);
        }
    }
}