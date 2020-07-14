namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IChangeCalculator
    {
        ChangeCalculatorResult CalculateChanges(IEnumerable<ITypeDefinition> oldTypes,
            IEnumerable<ITypeDefinition> newTypes, ComparerOptions options);
    }
}