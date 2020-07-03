﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System.Collections;
    using System.Collections.Generic;

    public class PropertyModifierDataSet : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {"", "", SemVerChangeType.None};
            yield return new object[] {"", "abstract", SemVerChangeType.Breaking};
            yield return new object[] {"", "new", SemVerChangeType.None};
            yield return new object[] {"", "override", SemVerChangeType.Feature};
            yield return new object[] {"", "sealed", SemVerChangeType.None};
            yield return new object[] {"", "static", SemVerChangeType.Breaking};
            yield return new object[] {"", "virtual", SemVerChangeType.Feature};
            yield return new object[] {"", "new abstract", SemVerChangeType.Breaking};
            yield return new object[] {"", "new static", SemVerChangeType.Breaking};
            yield return new object[] {"", "new virtual", SemVerChangeType.Feature};
            yield return new object[] {"", "sealed override", SemVerChangeType.None};
            yield return new object[] {"abstract", "", SemVerChangeType.Breaking};
            yield return new object[] {"abstract", "abstract", SemVerChangeType.None};
            yield return new object[] {"abstract", "new", SemVerChangeType.Breaking};
            yield return new object[] {"abstract", "override", SemVerChangeType.Feature};
            yield return new object[] {"abstract", "sealed", SemVerChangeType.Breaking};
            yield return new object[] {"abstract", "static", SemVerChangeType.Breaking};
            yield return new object[] {"abstract", "virtual", SemVerChangeType.Feature};
            yield return new object[] {"abstract", "new abstract", SemVerChangeType.None};
            yield return new object[] {"abstract", "new static", SemVerChangeType.Breaking};
            yield return new object[] {"abstract", "new virtual", SemVerChangeType.Feature};
            yield return new object[] {"abstract", "sealed override", SemVerChangeType.Breaking};
            yield return new object[] {"new", "", SemVerChangeType.None};
            yield return new object[] {"new", "abstract", SemVerChangeType.Breaking};
            yield return new object[] {"new", "new", SemVerChangeType.None};
            yield return new object[] {"new", "override", SemVerChangeType.Feature};
            yield return new object[] {"new", "sealed", SemVerChangeType.None};
            yield return new object[] {"new", "static", SemVerChangeType.Breaking};
            yield return new object[] {"new", "virtual", SemVerChangeType.Feature};
            yield return new object[] {"new", "new abstract", SemVerChangeType.Breaking};
            yield return new object[] {"new", "new static", SemVerChangeType.Breaking};
            yield return new object[] {"new", "new virtual", SemVerChangeType.Feature};
            yield return new object[] {"new", "sealed override", SemVerChangeType.None};
            yield return new object[] {"override", "", SemVerChangeType.Breaking};
            yield return new object[] {"override", "abstract", SemVerChangeType.Breaking};
            yield return new object[] {"override", "new", SemVerChangeType.Breaking};
            yield return new object[] {"override", "override", SemVerChangeType.None};
            yield return new object[] {"override", "sealed", SemVerChangeType.Breaking};
            yield return new object[] {"override", "static", SemVerChangeType.Breaking};
            yield return new object[] {"override", "virtual", SemVerChangeType.None};
            yield return new object[] {"override", "new abstract", SemVerChangeType.Breaking};
            yield return new object[] {"override", "new static", SemVerChangeType.Breaking};
            yield return new object[] {"override", "new virtual", SemVerChangeType.None};
            yield return new object[] {"override", "sealed override", SemVerChangeType.Breaking};
            yield return new object[] {"sealed", "", SemVerChangeType.None };
            yield return new object[] {"sealed", "abstract", SemVerChangeType.Breaking};
            yield return new object[] {"sealed", "new", SemVerChangeType.None};
            yield return new object[] {"sealed", "override", SemVerChangeType.Feature};
            yield return new object[] {"sealed", "sealed", SemVerChangeType.None};
            yield return new object[] {"sealed", "static", SemVerChangeType.Breaking};
            yield return new object[] {"sealed", "virtual", SemVerChangeType.Feature};
            yield return new object[] {"sealed", "new abstract", SemVerChangeType.Breaking};
            yield return new object[] {"sealed", "new static", SemVerChangeType.Breaking};
            yield return new object[] {"sealed", "new virtual", SemVerChangeType.Feature};
            yield return new object[] {"sealed", "sealed override", SemVerChangeType.None};
            yield return new object[] {"static", "", SemVerChangeType.Breaking};
            yield return new object[] {"static", "abstract", SemVerChangeType.Breaking};
            yield return new object[] {"static", "new", SemVerChangeType.Breaking};
            yield return new object[] {"static", "override", SemVerChangeType.Breaking};
            yield return new object[] {"static", "sealed", SemVerChangeType.Breaking};
            yield return new object[] {"static", "static", SemVerChangeType.None};
            yield return new object[] {"static", "virtual", SemVerChangeType.Breaking};
            yield return new object[] {"static", "new abstract", SemVerChangeType.Breaking};
            yield return new object[] {"static", "new static", SemVerChangeType.None};
            yield return new object[] {"static", "new virtual", SemVerChangeType.Breaking};
            yield return new object[] {"static", "sealed override", SemVerChangeType.Breaking};
            yield return new object[] {"virtual", "", SemVerChangeType.Breaking};
            yield return new object[] {"virtual", "abstract", SemVerChangeType.Breaking};
            yield return new object[] {"virtual", "new", SemVerChangeType.Breaking};
            yield return new object[] {"virtual", "override", SemVerChangeType.None};
            yield return new object[] {"virtual", "sealed", SemVerChangeType.Breaking};
            yield return new object[] {"virtual", "static", SemVerChangeType.Breaking};
            yield return new object[] {"virtual", "virtual", SemVerChangeType.None};
            yield return new object[] {"virtual", "new abstract", SemVerChangeType.Breaking};
            yield return new object[] {"virtual", "new static", SemVerChangeType.Breaking};
            yield return new object[] {"virtual", "new virtual", SemVerChangeType.None};
            yield return new object[] {"virtual", "sealed override", SemVerChangeType.Breaking};
            yield return new object[] {"new abstract", "", SemVerChangeType.Breaking};
            yield return new object[] {"new abstract", "abstract", SemVerChangeType.None};
            yield return new object[] {"new abstract", "new", SemVerChangeType.Breaking};
            yield return new object[] {"new abstract", "override", SemVerChangeType.Feature};
            yield return new object[] {"new abstract", "sealed", SemVerChangeType.Breaking};
            yield return new object[] {"new abstract", "static", SemVerChangeType.Breaking};
            yield return new object[] {"new abstract", "virtual", SemVerChangeType.Feature};
            yield return new object[] {"new abstract", "new abstract", SemVerChangeType.None};
            yield return new object[] {"new abstract", "new static", SemVerChangeType.Breaking};
            yield return new object[] {"new abstract", "new virtual", SemVerChangeType.Feature};
            yield return new object[] {"new abstract", "sealed override", SemVerChangeType.Breaking};
            yield return new object[] {"new static", "", SemVerChangeType.Breaking};
            yield return new object[] {"new static", "abstract", SemVerChangeType.Breaking};
            yield return new object[] {"new static", "new", SemVerChangeType.Breaking};
            yield return new object[] {"new static", "override", SemVerChangeType.Breaking};
            yield return new object[] {"new static", "sealed", SemVerChangeType.Breaking};
            yield return new object[] {"new static", "static", SemVerChangeType.None};
            yield return new object[] {"new static", "virtual", SemVerChangeType.Breaking};
            yield return new object[] {"new static", "new abstract", SemVerChangeType.Breaking};
            yield return new object[] {"new static", "new static", SemVerChangeType.None};
            yield return new object[] {"new static", "new virtual", SemVerChangeType.Breaking};
            yield return new object[] {"new static", "sealed override", SemVerChangeType.Breaking};
            yield return new object[] {"new virtual", "", SemVerChangeType.Breaking};
            yield return new object[] {"new virtual", "abstract", SemVerChangeType.Breaking};
            yield return new object[] {"new virtual", "new", SemVerChangeType.Breaking};
            yield return new object[] {"new virtual", "override", SemVerChangeType.None};
            yield return new object[] {"new virtual", "sealed", SemVerChangeType.Breaking};
            yield return new object[] {"new virtual", "static", SemVerChangeType.Breaking};
            yield return new object[] {"new virtual", "virtual", SemVerChangeType.None};
            yield return new object[] {"new virtual", "new abstract", SemVerChangeType.Breaking};
            yield return new object[] {"new virtual", "new static", SemVerChangeType.Breaking};
            yield return new object[] {"new virtual", "new virtual", SemVerChangeType.None};
            yield return new object[] {"new virtual", "sealed override", SemVerChangeType.Breaking};
            yield return new object[] {"sealed override", "", SemVerChangeType.None};
            yield return new object[] {"sealed override", "abstract", SemVerChangeType.Breaking};
            yield return new object[] {"sealed override", "new", SemVerChangeType.None};
            yield return new object[] {"sealed override", "override", SemVerChangeType.Feature};
            yield return new object[] {"sealed override", "sealed", SemVerChangeType.None};
            yield return new object[] {"sealed override", "static", SemVerChangeType.Breaking};
            yield return new object[] {"sealed override", "virtual", SemVerChangeType.Feature};
            yield return new object[] {"sealed override", "new abstract", SemVerChangeType.Breaking};
            yield return new object[] {"sealed override", "new static", SemVerChangeType.Breaking};
            yield return new object[] {"sealed override", "new virtual", SemVerChangeType.Feature};
            yield return new object[] {"sealed override", "sealed override", SemVerChangeType.None};
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}