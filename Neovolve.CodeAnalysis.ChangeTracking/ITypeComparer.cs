namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;

    public interface ITypeComparer
    {
        IEnumerable<ComparisonResult> CompareTypes(ItemMatch<ITypeDefinition> match, ComparerOptions options);
    }
}