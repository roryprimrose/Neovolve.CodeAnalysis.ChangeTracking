namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    /// <summary>
    ///     The <see cref="ArgumentType" />
    ///     enum identifies whether an argument is an ordinal argument or a named argument.
    /// </summary>
    public enum ArgumentType
    {
        /// <summary>
        ///     The argument is an ordinal argument in the attribute.
        /// </summary>
        Ordinal,

        /// <summary>
        ///     The argument is a named argument in the attribute.
        /// </summary>
        Named
    }
}