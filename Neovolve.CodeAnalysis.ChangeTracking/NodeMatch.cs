namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;

    public class NodeMatch
    {
        public NodeMatch(NodeDefinition oldDefinition, NodeDefinition newDefinition)
        {
            if (oldDefinition == null
                && newDefinition == null)
            {
                throw new ArgumentException("At least one NodeDefinition must be supplied.");
            }

            OldDefinition = oldDefinition;
            NewDefinition = newDefinition;
        }

        public NodeDefinition NewDefinition { get; }

        public NodeDefinition OldDefinition { get; }
    }
}