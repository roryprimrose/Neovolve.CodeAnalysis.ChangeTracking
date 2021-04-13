namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    /// <summary>
    ///     The <see cref="IPropertyDefinition" />
    ///     interface defines the members that describe a property.
    /// </summary>
    public interface IPropertyDefinition : IMemberDefinition, IModifiersElement<PropertyModifiers>
    {
        /// <summary>
        ///     Gets the property get accessor.
        /// </summary>
        IPropertyAccessorDefinition? GetAccessor { get; }

        /// <summary>
        ///     Gets the property set accessor.
        /// </summary>
        IPropertyAccessorDefinition? SetAccessor { get; }
    }
}