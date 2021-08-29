namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System.Collections;
    using System.Collections.Generic;

    public class EnumAccessModifiersDataSet : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            // @formatter:off — disable formatter after this line
            yield return new object[] { "", "", SemVerChangeType.None };
            yield return new object[] { "", "internal", SemVerChangeType.None };
            yield return new object[] { "", "private", SemVerChangeType.None };
            yield return new object[] { "", "protected", SemVerChangeType.Feature };
            yield return new object[] { "", "public", SemVerChangeType.Feature };
            yield return new object[] { "internal", "", SemVerChangeType.None };
            yield return new object[] { "internal", "internal", SemVerChangeType.None };
            yield return new object[] { "internal", "private", SemVerChangeType.None };
            yield return new object[] { "internal", "protected", SemVerChangeType.Feature };
            yield return new object[] { "internal", "public", SemVerChangeType.Feature };
            yield return new object[] { "private", "", SemVerChangeType.None };
            yield return new object[] { "private", "internal", SemVerChangeType.None };
            yield return new object[] { "private", "private", SemVerChangeType.None };
            yield return new object[] { "private", "protected", SemVerChangeType.Feature };
            yield return new object[] { "private", "public", SemVerChangeType.Feature };
            yield return new object[] { "protected", "", SemVerChangeType.Breaking };
            yield return new object[] { "protected", "internal", SemVerChangeType.Breaking };
            yield return new object[] { "protected", "private", SemVerChangeType.Breaking };
            yield return new object[] { "protected", "protected", SemVerChangeType.None };
            yield return new object[] { "protected", "public", SemVerChangeType.Feature };
            yield return new object[] { "public", "", SemVerChangeType.Breaking };
            yield return new object[] { "public", "internal", SemVerChangeType.Breaking };
            yield return new object[] { "public", "private", SemVerChangeType.Breaking };
            yield return new object[] { "public", "protected", SemVerChangeType.Breaking };
            yield return new object[] { "public", "public", SemVerChangeType.None };
            // @formatter:on — enable formatter after this line
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}