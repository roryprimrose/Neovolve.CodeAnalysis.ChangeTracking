namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System.Collections.Generic;
    using Xunit.Abstractions;

    internal static class TestOutputHelperExtensions
    {
        public static void WriteResult(this ITestOutputHelper output, ComparisonResult result)
        {
            output.WriteLine($"{result.ChangeType}: {result.Message}");
        }

        public static void WriteResults(this ITestOutputHelper output, IEnumerable<ComparisonResult> results)
        {
            foreach (var result in results)
            {
                WriteResult(output, result);
            }
        }
    }
}