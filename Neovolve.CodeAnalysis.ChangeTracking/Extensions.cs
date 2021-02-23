namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

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