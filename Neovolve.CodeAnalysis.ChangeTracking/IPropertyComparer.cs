namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;

    public interface IPropertyComparer
    {
        IEnumerable<ComparisonResult> CompareTypes(ItemMatch<PropertyDefinition> match, ComparerOptions options);
    }
}