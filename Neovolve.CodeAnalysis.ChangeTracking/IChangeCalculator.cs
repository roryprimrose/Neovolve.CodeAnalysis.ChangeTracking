namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IChangeCalculator" />
    ///     interface defines the members used to calculate semantic version changes between old and new types.
    /// </summary>
    public interface IChangeCalculator
    {
        /// <summary>
        ///     Calculates the changes identified between old and new types.
        /// </summary>
        /// <param name="oldTypes">The old types to evaluate.</param>
        /// <param name="newTypes">The new types to evaluate.</param>
        /// <param name="options">The comparison options to use when evaluating changes.</param>
        /// <returns>The changes identified between the old and new types.</returns>
        ChangeCalculatorResult CalculateChanges(IEnumerable<IBaseTypeDefinition> oldTypes,
            IEnumerable<IBaseTypeDefinition> newTypes, ComparerOptions options);
    }
}