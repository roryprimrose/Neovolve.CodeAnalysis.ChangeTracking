namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    /// <summary>
    ///     The <see cref="IPropertyDefinition" />
    ///     interface defines the members that describe a property.
    /// </summary>
    public interface IPropertyDefinition : IMemberDefinition
    {
        /// <summary>
        ///     Gets whether the property declares a getter.
        /// </summary>
        bool CanRead { get; }

        /// <summary>
        ///     Gets whether the property declares a setter.
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        ///     Gets the property modifiers.
        /// </summary>
        MemberModifiers Modifiers { get; }
    }
}