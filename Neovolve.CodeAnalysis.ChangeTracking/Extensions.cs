namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class Extensions
    {
        public static IList<T> FastToList<T>(this IEnumerable<T> values)
        {
            if (values is IList<T> list)
            {
                return list;
            }

            return values.ToList();
        }
    }
}