namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IMemberComparer
    {
        IEnumerable<ComparisonResult> CompareTypes(ItemMatch<IMemberDefinition> match, ComparerOptions options);
    }
}