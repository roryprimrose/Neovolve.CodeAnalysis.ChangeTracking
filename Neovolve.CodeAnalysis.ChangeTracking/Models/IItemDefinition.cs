namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    /// <summary>
    ///     The <see cref="IItemDefinition" />
    ///     interface defines common properties for code elements.
    /// </summary>
    public interface IItemDefinition
    {
        /// <summary>
        ///     Gets the type location.
        /// </summary>
        public DefinitionLocation Location { get; }

        /// <summary>
        ///     Gets the name of the item.
        /// </summary>
        public string Name { get; }
    }
}