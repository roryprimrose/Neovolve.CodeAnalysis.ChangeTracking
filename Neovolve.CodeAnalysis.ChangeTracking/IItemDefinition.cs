namespace Neovolve.CodeAnalysis.ChangeTracking
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
        ///     Gets the name of the type.
        /// </summary>
        public string Name { get; }
    }
}