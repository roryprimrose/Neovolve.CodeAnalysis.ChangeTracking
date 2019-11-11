namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using EnsureThat;
    using Newtonsoft.Json;

    internal static class Extensions
    {
        public static T JsonClone<T>(this T value) where T : class
        {
            Ensure.Any.IsNotNull(value, nameof(value));

            var data = JsonConvert.SerializeObject(value);

            return (T) JsonConvert.DeserializeObject(data, typeof(T));
        }
    }
}