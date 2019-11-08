namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using EnsureThat;

    public class NodeMatcher : INodeMatcher
    {
        public NodeMatch GetMatch(NodeDefinition oldDefinition, NodeDefinition newDefinition)
        {
            Ensure.Any.IsNotNull(oldDefinition, nameof(oldDefinition));
            Ensure.Any.IsNotNull(newDefinition, nameof(newDefinition));

            if (!string.Equals(oldDefinition.Namespace, newDefinition.Namespace, StringComparison.Ordinal))
            {
                return null;
            }

            if (!string.Equals(oldDefinition.OwningType, newDefinition.OwningType, StringComparison.Ordinal))
            {
                return null;
            }

            if (!string.Equals(oldDefinition.Name, newDefinition.Name, StringComparison.Ordinal))
            {
                return null;
            }

            return new NodeMatch(oldDefinition, newDefinition);
        }
    }
}