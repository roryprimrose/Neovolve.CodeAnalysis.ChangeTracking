namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class AttributeComparer : IAttributeComparer
    {
        public IEnumerable<ComparisonResult> CompareTypes(ItemMatch<IAttributeDefinition> match, ComparerOptions options)
        {
            yield break;
        }
    }
}