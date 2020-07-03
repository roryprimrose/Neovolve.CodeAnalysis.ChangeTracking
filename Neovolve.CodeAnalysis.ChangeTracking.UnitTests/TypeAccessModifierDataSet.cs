namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System.Collections;
    using System.Collections.Generic;

    public class TypeAccessModifierDataSet : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            // @formatter:off — disable formatter after this line
            yield return new object[] { "", "", SemVerChangeType.None};
            yield return new object[] { "", "internal", SemVerChangeType.None};
            yield return new object[] { "", "private", SemVerChangeType.None};
            yield return new object[] { "", "protected", SemVerChangeType.Feature};
            yield return new object[] { "", "public", SemVerChangeType.Feature};
            yield return new object[] { "", "protected internal", SemVerChangeType.Feature};
            yield return new object[] { "", "internal private", SemVerChangeType.None};
            yield return new object[] { "internal", "", SemVerChangeType.None };
            yield return new object[] { "internal", "internal", SemVerChangeType.None };
            yield return new object[] { "internal", "private", SemVerChangeType.None };
            yield return new object[] { "internal", "protected", SemVerChangeType.Feature };
            yield return new object[] { "internal", "public", SemVerChangeType.Feature };
            yield return new object[] { "internal", "protected internal", SemVerChangeType.Feature };
            yield return new object[] { "internal", "internal private", SemVerChangeType.None };
            yield return new object[] { "private", "", SemVerChangeType.None };
            yield return new object[] { "private", "internal", SemVerChangeType.None };
            yield return new object[] { "private", "private", SemVerChangeType.None };
            yield return new object[] { "private", "protected", SemVerChangeType.Feature };
            yield return new object[] { "private", "public", SemVerChangeType.Feature };
            yield return new object[] { "private", "protected internal", SemVerChangeType.Feature };
            yield return new object[] { "private", "internal private", SemVerChangeType.None };
            yield return new object[] { "protected", "", SemVerChangeType.Breaking };
            yield return new object[] { "protected", "internal", SemVerChangeType.Breaking };
            yield return new object[] { "protected", "private", SemVerChangeType.Breaking };
            yield return new object[] { "protected", "protected", SemVerChangeType.None };
            yield return new object[] { "protected", "public", SemVerChangeType.Feature };
            yield return new object[] { "protected", "protected internal", SemVerChangeType.None };
            yield return new object[] { "protected", "internal private", SemVerChangeType.Breaking };
            yield return new object[] { "public", "", SemVerChangeType.Breaking };
            yield return new object[] { "public", "internal", SemVerChangeType.Breaking };
            yield return new object[] { "public", "private", SemVerChangeType.Breaking };
            yield return new object[] { "public", "protected", SemVerChangeType.Breaking };
            yield return new object[] { "public", "public", SemVerChangeType.None };
            yield return new object[] { "public", "protected internal", SemVerChangeType.Breaking };
            yield return new object[] { "public", "internal private", SemVerChangeType.Breaking };
            yield return new object[] { "protected internal", "", SemVerChangeType.Breaking };
            yield return new object[] { "protected internal", "internal", SemVerChangeType.Breaking };
            yield return new object[] { "protected internal", "private", SemVerChangeType.Breaking };
            yield return new object[] { "protected internal", "protected", SemVerChangeType.None };
            yield return new object[] { "protected internal", "public", SemVerChangeType.Feature };
            yield return new object[] { "protected internal", "protected internal", SemVerChangeType.None };
            yield return new object[] { "protected internal", "internal private", SemVerChangeType.Breaking };
            yield return new object[] { "internal private", "", SemVerChangeType.None };
            yield return new object[] { "internal private", "internal", SemVerChangeType.None };
            yield return new object[] { "internal private", "private", SemVerChangeType.None };
            yield return new object[] { "internal private", "protected", SemVerChangeType.Feature };
            yield return new object[] { "internal private", "public", SemVerChangeType.Feature };
            yield return new object[] { "internal private", "protected internal", SemVerChangeType.Feature };
            yield return new object[] { "internal private", "internal private", SemVerChangeType.None };
            // @formatter:on — enable formatter after this line
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}