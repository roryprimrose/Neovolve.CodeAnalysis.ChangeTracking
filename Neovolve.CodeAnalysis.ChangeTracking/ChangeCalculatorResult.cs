namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;

    public class ChangeCalculatorResult
    {
        public void Add(ComparisonResult result)
        {
            result = result ?? throw new ArgumentNullException(nameof(result));

            if (ChangeType < result.ChangeType)
            {
                ChangeType = result.ChangeType;
            }

            ComparisonResults.Add(result);
        }

        public SemVerChangeType ChangeType { get; private set; } = SemVerChangeType.None;

        public ICollection<ComparisonResult> ComparisonResults { get; } = new List<ComparisonResult>();
    }
}