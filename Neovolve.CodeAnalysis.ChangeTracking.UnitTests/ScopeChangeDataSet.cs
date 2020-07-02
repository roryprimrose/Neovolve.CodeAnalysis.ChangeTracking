namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System.Collections;
    using System.Collections.Generic;

    public class ScopeChangeDataSet : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            // Combinatorics calculated by https://www.mathsisfun.com/combinatorics/combinations-permutations-calculator.html
            yield return new object[] { "","", SemVerChangeType.None };
            yield return new object[] { "","public", SemVerChangeType.Feature };
            yield return new object[] { "","private", SemVerChangeType.None };
            yield return new object[] { "","internal", SemVerChangeType.None };
            yield return new object[] { "","protected", SemVerChangeType.Feature };
            yield return new object[] { "","private protected", SemVerChangeType.Feature };
            yield return new object[] { "","protected internal", SemVerChangeType.Feature };
            yield return new object[] { "public","", SemVerChangeType.Breaking };
            yield return new object[] { "public","public", SemVerChangeType.None };
            yield return new object[] { "public","private", SemVerChangeType.Breaking };
            yield return new object[] { "public","internal", SemVerChangeType.Breaking };
            yield return new object[] { "public","protected", SemVerChangeType.None };
            yield return new object[] { "public","private protected", SemVerChangeType.None };
            yield return new object[] { "public","protected internal", SemVerChangeType.None };
            yield return new object[] { "private","", SemVerChangeType.None };
            yield return new object[] { "private","public", SemVerChangeType.Feature };
            yield return new object[] { "private","private", SemVerChangeType.None };
            yield return new object[] { "private","internal", SemVerChangeType.None };
            yield return new object[] { "private","protected", SemVerChangeType.Feature };
            yield return new object[] { "private","private protected", SemVerChangeType.Feature };
            yield return new object[] { "private","protected internal", SemVerChangeType.Feature };
            yield return new object[] { "internal","", SemVerChangeType.None };
            yield return new object[] { "internal","public", SemVerChangeType.Feature };
            yield return new object[] { "internal","private", SemVerChangeType.None };
            yield return new object[] { "internal","internal", SemVerChangeType.None };
            yield return new object[] { "internal","protected", SemVerChangeType.Feature };
            yield return new object[] { "internal","private protected", SemVerChangeType.Feature };
            yield return new object[] { "internal","protected internal", SemVerChangeType.Feature };
            yield return new object[] { "protected","", SemVerChangeType.Breaking };
            yield return new object[] { "protected","public", SemVerChangeType.None };
            yield return new object[] { "protected","private", SemVerChangeType.Breaking };
            yield return new object[] { "protected","internal", SemVerChangeType.Breaking };
            yield return new object[] { "protected","protected", SemVerChangeType.None };
            yield return new object[] { "protected","private protected", SemVerChangeType.None };
            yield return new object[] { "protected","protected internal", SemVerChangeType.None };
            yield return new object[] { "private protected","", SemVerChangeType.Breaking };
            yield return new object[] { "private protected","public", SemVerChangeType.None };
            yield return new object[] { "private protected","private", SemVerChangeType.Breaking };
            yield return new object[] { "private protected","internal", SemVerChangeType.Breaking };
            yield return new object[] { "private protected","protected", SemVerChangeType.None };
            yield return new object[] { "private protected","private protected", SemVerChangeType.None };
            yield return new object[] { "private protected","protected internal", SemVerChangeType.None };
            yield return new object[] { "protected internal","", SemVerChangeType.Breaking };
            yield return new object[] { "protected internal","public", SemVerChangeType.None };
            yield return new object[] { "protected internal","private", SemVerChangeType.Breaking };
            yield return new object[] { "protected internal","internal", SemVerChangeType.Breaking };
            yield return new object[] { "protected internal","protected", SemVerChangeType.None };
            yield return new object[] { "protected internal","private protected", SemVerChangeType.None };
            yield return new object[] { "protected internal","protected internal", SemVerChangeType.None };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}