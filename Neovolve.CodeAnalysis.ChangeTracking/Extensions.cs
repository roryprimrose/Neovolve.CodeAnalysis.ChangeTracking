namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class Extensions
    {
        public static List<T> FastToList<T>(this IEnumerable<T> values)
        {
            if (values is List<T> list)
            {
                return list;
            }

            return values.ToList();
        }
    }
}