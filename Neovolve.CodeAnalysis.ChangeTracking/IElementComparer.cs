namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IElementComparer<T> where T : IElementDefinition
    {
        IEnumerable<ComparisonResult> CompareItems(ItemMatch<T> match, ComparerOptions options);
    }
}