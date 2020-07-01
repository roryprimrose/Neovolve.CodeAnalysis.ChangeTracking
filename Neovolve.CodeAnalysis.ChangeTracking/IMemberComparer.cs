namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;

    public interface IMemberComparer
    {
        IEnumerable<ComparisonResult> CompareTypes(ItemMatch<IMemberDefinition> match, ComparerOptions options);
    }
}