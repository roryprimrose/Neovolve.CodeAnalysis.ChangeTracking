namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using EnsureThat;
    using Newtonsoft.Json;

    internal static class Extensions
    {
        public static T JsonClone<T>(this T value) where T : class
        {
            Ensure.Any.IsNotNull(value, nameof(value));

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