namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;

    public interface IChangeCalculator
    {
        ChangeCalculatorResult CalculateChanges(IEnumerable<ITypeDefinition> oldTypes,
            IEnumerable<ITypeDefinition> newTypes, ComparerOptions options);
    }
}