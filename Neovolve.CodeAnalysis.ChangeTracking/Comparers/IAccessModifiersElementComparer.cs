namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IAccessModifiersElementComparer<T> where T : struct, Enum
    {
        IEnumerable<ComparisonResult> CompareItems(
            ItemMatch<IAccessModifiersElement<T>> match,
            ComparerOptions options);
    }
}