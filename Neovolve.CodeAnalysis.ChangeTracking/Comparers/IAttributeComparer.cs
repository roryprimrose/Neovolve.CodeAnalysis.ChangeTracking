namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IAttributeComparer
    {
        IEnumerable<ComparisonResult> CompareItems(ItemMatch<IAttributeDefinition> match, ComparerOptions options);
    }
}