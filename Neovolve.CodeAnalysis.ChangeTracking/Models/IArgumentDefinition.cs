namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    /// <summary>
    ///     The <see cref="IArgumentDefinition" />
    ///     class defines the members that describe an attribute.
    /// </summary>
    public interface IArgumentDefinition : IItemDefinition
    {
        /// <summary>
        ///     Gets the type of argument.
        /// </summary>
        ArgumentType ArgumentType { get; }

        /// <summary>
        ///     Gets the ordinal index for where the argument exists in the list of arguments.
        /// </summary>
        public int? OrdinalIndex { get; }

        /// <summary>
        ///     Gets the value of the argument.
        /// </summary>
        string Value { get; }
    }
}