namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System.Collections;
    using System.Collections.Generic;

    public class PropertyAccessorAccessModifierDataSet : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            // @formatter:off — disable formatter after this line
            yield return new object[] { "", "", SemVerChangeType.None };
            yield return new object[] { "", "internal", SemVerChangeType.None };
            yield return new object[] { "", "private", SemVerChangeType.None };
            yield return new object[] { "", "protected", SemVerChangeType.Feature };
            yield return new object[] { "", "protected internal", SemVerChangeType.Feature };
            yield return new object[] { "internal", "", SemVerChangeType.None };
            yield return new object[] { "internal", "internal", SemVerChangeType.None };
            yield return new object[] { "internal", "private", SemVerChangeType.None };
            yield return new object[] { "internal", "protected", SemVerChangeType.Feature };
            yield return new object[] { "internal", "protected internal", SemVerChangeType.Feature };
            yield return new object[] { "private", "", SemVerChangeType.None };
            yield return new object[] { "private", "internal", SemVerChangeType.None };
            yield return new object[] { "private", "private", SemVerChangeType.None };
            yield return new object[] { "private", "protected", SemVerChangeType.Feature };
            yield return new object[] { "private", "protected internal", SemVerChangeType.Feature };
            yield return new object[] { "protected", "", SemVerChangeType.Breaking };
            yield return new object[] { "protected", "internal", SemVerChangeType.Breaking };
            yield return new object[] { "protected", "private", SemVerChangeType.Breaking };
            yield return new object[] { "protected", "protected", SemVerChangeType.None };
            yield return new object[] { "protected", "protected internal", SemVerChangeType.None };
            yield return new object[] { "protected internal", "", SemVerChangeType.Breaking };
            yield return new object[] { "protected internal", "internal", SemVerChangeType.Breaking };
            yield return new object[] { "protected internal", "private", SemVerChangeType.Breaking };
            yield return new object[] { "protected internal", "protected", SemVerChangeType.None };
            yield return new object[] { "protected internal", "protected internal", SemVerChangeType.None };
            // @formatter:on — enable formatter after this line
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}