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
        ///     Gets whether the property is an abstract property.
        /// </summary>
        bool IsAbstract { get; }

        /// <summary>
        ///     Gets whether the property is a new property.
        /// </summary>
        bool IsNew { get; }

        /// <summary>
        ///     Gets whether the property is overriding the base class property.
        /// </summary>
        bool IsOverride { get; }

        /// <summary>
        ///     Gets whether the property is a sealed property.
        /// </summary>
        bool IsSealed { get; }

        /// <summary>
        ///     Gets whether the property is a static property.
        /// </summary>
        bool IsStatic { get; }

        /// <summary>
        ///     Gets whether the property is a virtual property.
        /// </summary>
        bool IsVirtual { get; }
    }
}