﻿namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public interface INodeMatcher
    {
        NodeMatch GetMatch(NodeDefinition oldNode, NodeDefinition newNode);

        bool IsSupported(NodeDefinition node);
    }
}