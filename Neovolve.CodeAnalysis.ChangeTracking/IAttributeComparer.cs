namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IAttributeComparer
    {
        IEnumerable<ComparisonResult> CompareTypes(ItemMatch<IAttributeDefinition> match, ComparerOptions options);
    }
}