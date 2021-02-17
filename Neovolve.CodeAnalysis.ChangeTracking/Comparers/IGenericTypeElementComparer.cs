namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IGenericTypeElementComparer
    {
        IEnumerable<ComparisonResult> CompareItems(
            ItemMatch<IGenericTypeElement> match,
            ComparerOptions options);
    }
}