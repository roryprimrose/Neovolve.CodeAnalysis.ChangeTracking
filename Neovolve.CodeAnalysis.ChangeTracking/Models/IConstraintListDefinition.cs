namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="IConstraintListDefinition" />
    ///     interface defines the members that describe a generic constraint list.
    /// </summary>
    public interface IConstraintListDefinition : IItemDefinition
    {
        /// <summary>
        ///     Gets the constraints declared for the generic type definition.
        /// </summary>
        IReadOnlyCollection<string> Constraints { get; }
    }
}