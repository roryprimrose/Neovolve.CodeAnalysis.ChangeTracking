namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface ITypeComparer
    {
        IEnumerable<ComparisonResult> CompareTypes(ItemMatch<ITypeDefinition> match, ComparerOptions options);
    }
}