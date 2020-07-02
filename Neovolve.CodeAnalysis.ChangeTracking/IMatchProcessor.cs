namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IMatchProcessor<in T> where T : class, IItemDefinition
    {
        IEnumerable<ComparisonResult> CalculateChanges(IEnumerable<T> oldItems,
            IEnumerable<T> newItems, ComparerOptions options);
    }
}