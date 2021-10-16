namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IMatchProcessor{T}"/>
    /// interface defines the members for processing matches between old and new items to calculate comparison results.
    /// </summary>
    /// <typeparam name="T">The type of item to process.</typeparam>
    public interface IMatchProcessor<in T> where T : IItemDefinition
    {
        IEnumerable<ComparisonResult> CalculateChanges(IEnumerable<T> oldItems,
            IEnumerable<T> newItems, ComparerOptions options);
    }
}