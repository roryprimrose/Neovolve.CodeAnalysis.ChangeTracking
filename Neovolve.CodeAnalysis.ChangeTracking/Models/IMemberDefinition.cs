namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    /// <summary>
    ///     The <see cref="IMemberDefinition" />
    ///     interface defines common properties for members.
    /// </summary>
    public interface IMemberDefinition : IElementDefinition
    {
        /// <summary>
        ///     Gets the type that declares the member.
        /// </summary>
        public ITypeDefinition DeclaringType { get; }

        /// <summary>
        ///     Gets the type that the member returns.
        /// </summary>
        public string ReturnType { get; }
    }
}