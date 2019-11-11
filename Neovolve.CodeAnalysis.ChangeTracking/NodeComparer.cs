namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Diagnostics;
    using EnsureThat;

    public class NodeComparer : INodeComparer
    {
        public virtual ChangeType Compare(NodeDefinition oldNode, NodeDefinition newNode)
        {
            Ensure.Any.IsNotNull(oldNode, nameof(oldNode));
            Ensure.Any.IsNotNull(newNode, nameof(newNode));

            if (string.Equals(oldNode.Namespace, newNode.Namespace, StringComparison.Ordinal) == false)
            {
                throw new InvalidOperationException("The two nodes cannot be compared because they have different Namespace values.");
            }
            
            if (string.Equals(oldNode.OwningType, newNode.OwningType, StringComparison.Ordinal) == false)
            {
                throw new InvalidOperationException("The two nodes cannot be compared because they have different OwningType values.");
            }
            
            if (string.Equals(oldNode.Name, newNode.Name, StringComparison.Ordinal) == false)
            {
                throw new InvalidOperationException("The two nodes cannot be compared because they have different Name values.");
            }

            if (oldNode.IsPublic == false
                && newNode.IsPublic == false)
            {
                // It doesn't matter if there is a change to the return type, the node isn't visible anyway
                return ChangeType.None;
            }

            if (oldNode.IsPublic
                && newNode.IsPublic == false)
            {
                // The node was public but isn't now, breaking change
                return ChangeType.Breaking;
            }

            if (oldNode.IsPublic == false
                && newNode.IsPublic)
            {
                // The node return type may have changed, but the node is only now becoming public
                // This is a feature because the public API didn't break even if the return type has changed
                return ChangeType.Feature;
            }

            Debug.Assert(oldNode.IsPublic);
            Debug.Assert(newNode.IsPublic);

            // At this point both the old node and the new node are public
            if (oldNode.ReturnType.Equals(newNode.ReturnType, StringComparison.Ordinal) == false)
            {
                return ChangeType.Breaking;
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