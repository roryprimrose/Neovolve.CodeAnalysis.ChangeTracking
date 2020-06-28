namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System.Collections;
    using System.Collections.Generic;

    public class GrandparentHierarchyIsVisibleDataSet : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            // These test scenarios were calculated by the cool combinatorics calculator at
            // https://www.mathsisfun.com/combinatorics/combinations-permutations-calculator.html
            yield return new object[] { "internal", "internal", "internal", false };
            yield return new object[] { "internal", "internal", "private", false };
            yield return new object[] { "internal", "internal", "protected", false };
            yield return new object[] { "internal", "internal", "public", false };
            yield return new object[] { "internal", "private", "internal", false };
            yield return new object[] { "internal", "private", "private", false };
            yield return new object[] { "internal", "private", "protected", false };
            yield return new object[] { "internal", "private", "public", false };
            yield return new object[] { "internal", "protected", "internal", false };
            yield return new object[] { "internal", "protected", "private", false };
            yield return new object[] { "internal", "protected", "protected", false };
            yield return new object[] { "internal", "protected", "public", false };
            yield return new object[] { "internal", "public", "internal", false };
            yield return new object[] { "internal", "public", "private", false };
            yield return new object[] { "internal", "public", "protected", false };
            yield return new object[] { "internal", "public", "public", false };
            yield return new object[] { "private", "internal", "internal", false };
            yield return new object[] { "private", "internal", "private", false };
            yield return new object[] { "private", "internal", "protected", false };
            yield return new object[] { "private", "internal", "public", false };
            yield return new object[] { "private", "private", "internal", false };
            yield return new object[] { "private", "private", "private", false };
            yield return new object[] { "private", "private", "protected", false };
            yield return new object[] { "private", "private", "public", false };
            yield return new object[] { "private", "protected", "internal", false };
            yield return new object[] { "private", "protected", "private", false };
            yield return new object[] { "private", "protected", "protected", false };
            yield return new object[] { "private", "protected", "public", false };
            yield return new object[] { "private", "public", "internal", false };
            yield return new object[] { "private", "public", "private", false };
            yield return new object[] { "private", "public", "protected", false };
            yield return new object[] { "private", "public", "public", false };
            yield return new object[] { "protected", "internal", "internal", false };
            yield return new object[] { "protected", "internal", "private", false };
            yield return new object[] { "protected", "internal", "protected", false };
            yield return new object[] { "protected", "internal", "public", false };
            yield return new object[] { "protected", "private", "internal", false };
            yield return new object[] { "protected", "private", "private", false };
            yield return new object[] { "protected", "private", "protected", false };
            yield return new object[] { "protected", "private", "public", false };
            yield return new object[] { "protected", "protected", "internal", false };
            yield return new object[] { "protected", "protected", "private", false };
            yield return new object[] { "protected", "protected", "protected", true };
            yield return new object[] { "protected", "protected", "public", true };
            yield return new object[] { "protected", "public", "internal", false };
            yield return new object[] { "protected", "public", "private", false };
            yield return new object[] { "protected", "public", "protected", true };
            yield return new object[] { "protected", "public", "public", true };
            yield return new object[] { "public", "internal", "internal", false };
            yield return new object[] { "public", "internal", "private", false };
            yield return new object[] { "public", "internal", "protected", false };
            yield return new object[] { "public", "internal", "public", false };
            yield return new object[] { "public", "private", "internal", false };
            yield return new object[] { "public", "private", "private", false };
            yield return new object[] { "public", "private", "protected", false };
            yield return new object[] { "public", "private", "public", false };
            yield return new object[] { "public", "protected", "internal", false };
            yield return new object[] { "public", "protected", "private", false };
            yield return new object[] { "public", "protected", "protected", true };
            yield return new object[] { "public", "protected", "public", true };
            yield return new object[] { "public", "public", "internal", false };
            yield return new object[] { "public", "public", "private", false };
            yield return new object[] { "public", "public", "protected", true };
            yield return new object[] { "public", "public", "public", true };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}