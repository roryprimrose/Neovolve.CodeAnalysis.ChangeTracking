namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class EnumCombinationsDataSet<T> : IEnumerable<object[]> where T : struct, Enum
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var values = Enum.GetValues(typeof(T)).OfType<T>().ToList();

            for (var outer = 0; outer < values.Count; outer++)
            {
                for (var inner = 0; inner < values.Count; inner++)
                {
                    yield return new object[] { values[outer], values[inner] };
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}