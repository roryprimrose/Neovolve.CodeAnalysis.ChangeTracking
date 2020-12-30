namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IElementMatchProcessor<in T> where T : IElementDefinition
    {
        IEnumerable<ComparisonResult> CalculateChanges(IEnumerable<T> oldItems,
            IEnumerable<T> newItems, ComparerOptions options);
    }
}