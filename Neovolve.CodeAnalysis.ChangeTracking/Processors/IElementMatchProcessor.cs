namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IElementMatchProcessor{T}"/>
    /// interface defines the members for processing matches between old and new elements to calculate comparison results.
    /// </summary>
    public interface IElementMatchProcessor<in T> where T : IElementDefinition
    {
        IEnumerable<ComparisonResult> CalculateChanges(IEnumerable<T> oldItems,
            IEnumerable<T> newItems, ComparerOptions options);
    }
}