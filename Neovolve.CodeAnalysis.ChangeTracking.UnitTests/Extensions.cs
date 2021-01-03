namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    internal static class Extensions
    {
        public static void ForceEnumeration<T>(this IEnumerable<T> source)
        {
            foreach (var item in source)
            {
            }
        }

        public static T JsonClone<T>(this T value) where T : class
        {
            var data = JsonConvert.SerializeObject(value);

            var type = typeof(T);

            if (JsonConvert.DeserializeObject(data, type) is T result)
            {
                return result;
            }

            throw new InvalidOperationException("Failed to copy " + typeof(T).FullName + " using json clone");
        }
    }
}