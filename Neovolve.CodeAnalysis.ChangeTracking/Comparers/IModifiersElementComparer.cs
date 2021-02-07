namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IModifiersElementComparer<T> where T : struct, Enum
    {
        IEnumerable<ComparisonResult> CompareItems(
            ItemMatch<IModifiersElement<T>> match,
            ComparerOptions options);
    }
}