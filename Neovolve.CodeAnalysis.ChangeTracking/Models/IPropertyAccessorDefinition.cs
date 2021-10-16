namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    /// <summary>
    ///     The <see cref="IPropertyAccessorDefinition" />
    ///     interface defines the members that describe a property accessor.
    /// </summary>
    public interface IPropertyAccessorDefinition : IAccessModifiersElement<PropertyAccessorAccessModifiers>
    {
        /// <summary>
        ///     Gets the purpose of the property accessor.
        /// </summary>
        PropertyAccessorPurpose AccessorPurpose { get; }

        /// <summary>
        ///     Gets the type of property accessor.
        /// </summary>
        PropertyAccessorType AccessorType { get; }
    }
}