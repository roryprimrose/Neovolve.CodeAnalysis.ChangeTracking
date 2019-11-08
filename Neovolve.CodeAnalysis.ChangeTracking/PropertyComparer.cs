namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using EnsureThat;

    public class PropertyComparer : NodeComparer
    {
        public override ChangeType Compare(NodeDefinition oldNode, NodeDefinition newNode)
        {
            Ensure.Any.IsNotNull(oldNode, nameof(oldNode));
            Ensure.Type.IsOfType(oldNode, typeof(PropertyDefinition), nameof(oldNode));
            Ensure.Any.IsNotNull(newNode, nameof(newNode));
            Ensure.Type.IsOfType(newNode, typeof(PropertyDefinition), nameof(newNode));

            var oldProperty = (PropertyDefinition)oldNode;
            var newProperty = (PropertyDefinition)newNode;

            var changeType = base.Compare(oldProperty, newProperty);

            if (changeType == ChangeType.Breaking)
            {
                // Doesn't matter if the property accessibility indicates feature or no change, breaking trumps everything
                return changeType;
            }

            if (oldProperty.IsPublic == false
                && newProperty.IsPublic == false)
            {
                // It doesn't matter if the accessibility of get/set has changed because the properties aren't visible anyway
                return changeType;
            }

            if (oldProperty.CanRead == newProperty.CanRead
                && oldProperty.CanWrite == newProperty.CanWrite)
            {
                // The accessibility of the property get/set members are equal so the changeType already calculated will be accurate
                return changeType;
            }

            // Calculate breaking changes
            if (oldProperty.CanRead
                && newProperty.CanRead == false)
            {
                return ChangeType.Breaking;
            }
            
            if (oldProperty.CanWrite
                && newProperty.CanWrite == false)
            {
                return ChangeType.Breaking;
            }
            
            // Calculate feature changes
            if (oldProperty.CanRead == false
                && newProperty.CanRead)
            {
                return ChangeType.Feature;
            }
            
            // Only other possible scenario at this point is that the old property couldn't write but the new property can
            return ChangeType.Feature;
        }

        public override bool IsSupported(NodeDefinition node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            return node.GetType() == typeof(PropertyDefinition);
        }
    }
}