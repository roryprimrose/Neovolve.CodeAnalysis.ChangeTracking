namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    /// <summary>
    ///     The <see cref="IEnumMemberDefinition" />
    ///     interface defines the members that describe an enum member.
    /// </summary>
    public interface IEnumMemberDefinition : IElementDefinition
    {
        /// <summary>
        ///     Gets or sets the type that declares the member.
        /// </summary>
        public IEnumDefinition DeclaringType { get; set; }

        /// <summary>
        ///     Gets the index of the member declaration in the enum.
        /// </summary>
        public int Index { get; }

        /// <summary>
        ///     Gets or sets the value of the member.
        /// </summary>
        public string Value { get; }
    }
}