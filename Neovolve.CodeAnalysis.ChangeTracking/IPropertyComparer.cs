namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IPropertyComparer
    {
        IEnumerable<ComparisonResult> CompareTypes(ItemMatch<IPropertyDefinition> match, ComparerOptions options);
    }
}