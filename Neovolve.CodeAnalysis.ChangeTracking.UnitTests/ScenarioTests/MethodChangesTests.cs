namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ScenarioTests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Xunit;
    using Xunit.Abstractions;

    public class MethodChangesTests
    {
        private readonly IChangeCalculator _calculator;

        public MethodChangesTests(ITestOutputHelper output)
        {
            var logger = output.BuildLogger(LogLevel.Information);

            _calculator = ChangeCalculatorFactory.BuildCalculator(logger);
        }

        [Theory]
        [ClassData(typeof(AccessModifiersDataSet))]
        public async Task EvaluatesChangeOfAccessModifiers(
            string oldModifiers,
            string newModifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new(NoParameters.Replace("public string MyMethod", oldModifiers + " string MyMethod"))
            };
            var newCode = new List<CodeSource>
            {
                new(NoParameters.Replace("public string MyMethod", newModifiers + " string MyMethod"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [InlineData("", "", AttributeCompareOption.All, SemVerChangeType.None)]
        [InlineData("", "", AttributeCompareOption.ByExpression, SemVerChangeType.None)]
        [InlineData("", "", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("", "[Custom]", AttributeCompareOption.All, SemVerChangeType.Feature)]
        [InlineData("", "[Custom]", AttributeCompareOption.ByExpression, SemVerChangeType.None)]
        [InlineData("", "[Custom]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[Custom]", "", AttributeCompareOption.All, SemVerChangeType.Breaking)]
        [InlineData("[Custom]", "", AttributeCompareOption.ByExpression, SemVerChangeType.None)]
        [InlineData("[Custom]", "", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[Custom]", "[Custom(\"SomeName\", true, 123]", AttributeCompareOption.All,
            SemVerChangeType.Breaking)]
        [InlineData("[Custom]", "[Custom(\"SomeName\", true, 123]", AttributeCompareOption.ByExpression,
            SemVerChangeType.None)]
        [InlineData("[Custom]", "[Custom(\"SomeName\", true, 123]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[Custom(\"SomeName\", true, 123]", "[Custom]", AttributeCompareOption.All,
            SemVerChangeType.Breaking)]
        [InlineData("[Custom(\"SomeName\", true, 123]", "[Custom]", AttributeCompareOption.ByExpression,
            SemVerChangeType.None)]
        [InlineData("[Custom(\"SomeName\", true, 123]", "[Custom]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[Custom][Another]", "[Custom][Another]", AttributeCompareOption.All, SemVerChangeType.None)]
        [InlineData("[Custom][Another]", "[Custom][Another]", AttributeCompareOption.ByExpression,
            SemVerChangeType.None)]
        [InlineData("[Custom][Another]", "[Custom][Another]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[Custom][Another]", "[Custom, Another]", AttributeCompareOption.All, SemVerChangeType.None)]
        [InlineData("[Custom][Another]", "[Custom, Another]", AttributeCompareOption.ByExpression,
            SemVerChangeType.None)]
        [InlineData("[Custom][Another]", "[Custom, Another]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[Custom, Another]", "[Custom][Another]", AttributeCompareOption.All, SemVerChangeType.None)]
        [InlineData("[Custom, Another]", "[Custom][Another]", AttributeCompareOption.ByExpression,
            SemVerChangeType.None)]
        [InlineData("[Custom, Another]", "[Custom][Another]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("", "[JsonConverter]", AttributeCompareOption.All, SemVerChangeType.Feature)]
        [InlineData("", "[JsonConverter]", AttributeCompareOption.ByExpression, SemVerChangeType.Feature)]
        [InlineData("", "[JsonConverter]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[JsonConverter]", "", AttributeCompareOption.All, SemVerChangeType.Breaking)]
        [InlineData("[JsonConverter]", "", AttributeCompareOption.ByExpression, SemVerChangeType.Breaking)]
        [InlineData("[JsonConverter]", "", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[JsonConverter]", "[JsonConverter(\"SomeName\"]", AttributeCompareOption.All,
            SemVerChangeType.Breaking)]
        [InlineData("[JsonConverter]", "[JsonConverter(\"SomeName\"]", AttributeCompareOption.ByExpression,
            SemVerChangeType.Breaking)]
        [InlineData("[JsonConverter]", "[JsonConverter(\"SomeName\"]", AttributeCompareOption.Skip,
            SemVerChangeType.None)]
        [InlineData("[JsonConverter(\"SomeName\"]", "[JsonConverter]", AttributeCompareOption.All,
            SemVerChangeType.Breaking)]
        [InlineData("[JsonConverter(\"SomeName\"]", "[JsonConverter]", AttributeCompareOption.ByExpression,
            SemVerChangeType.Breaking)]
        [InlineData("[JsonConverter(\"SomeName\"]", "[JsonConverter]", AttributeCompareOption.Skip,
            SemVerChangeType.None)]
        public async Task EvaluatesChangeOfMethodAttribute(
            string oldAttribute,
            string newAttribute,
            AttributeCompareOption compareOptions,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new(NoParameters.Replace("public string MyMethod", oldAttribute + " public string MyMethod"))
            };
            var newCode = new List<CodeSource>
            {
                new(NoParameters.Replace("public string MyMethod", newAttribute + " public string MyMethod"))
            };

            var options = OptionsFactory.BuildOptions();

            options.CompareAttributes = compareOptions;

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [ClassData(typeof(MethodModifierDataSet))]
        public async Task EvaluatesChangeOfMethodModifiers(
            string oldModifiers,
            string newModifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new(NoParameters.Replace("public string MyMethod", "public " + oldModifiers + " string MyMethod"))
            };
            var newCode = new List<CodeSource>
            {
                new(NoParameters.Replace("public string MyMethod", "public " + newModifiers + " string MyMethod"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [InlineData("MyMethod<T>(T first)", "MyMethod<T>(T first)", SemVerChangeType.None)]
        [InlineData("MyMethod<T>(T first)", "MyMethod<V>(V first)", SemVerChangeType.None)]
        [InlineData("MyMethod<T>()", "MyMethod<T, V>()", SemVerChangeType.Breaking)]
        [InlineData("MyMethod<T, V>()", "MyMethod<T>()", SemVerChangeType.Breaking)]
        [InlineData("MyMethod<T>(T first, V)", "MyMethod<T, V>(T first, V second)", SemVerChangeType.Breaking)]
        [InlineData("MyMethod<T, V>(T first, V second)", "MyMethod<T>(T first)", SemVerChangeType.Breaking)]
        public async Task EvaluatesChangeOfGenericTypeDefinitions(
            string oldSignature,
            string newSignature,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new(NoParameters.Replace("public string MyMethod()", "public string " + oldSignature))
            };
            var newCode = new List<CodeSource>
            {
                new(NoParameters.Replace("public string MyMethod()", "public string " + newSignature))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [InlineData("MyMethod()", "MyMethod()", SemVerChangeType.None)]
        [InlineData("MyMethod()", "MyOtherMethod()", SemVerChangeType.Breaking)]
        [InlineData("MyMethod(string first, bool second, int third)", "MyOtherMethod(string first, bool second, int third)", SemVerChangeType.Breaking)]
        [InlineData("MyMethod(string first, bool second, int third)", "MyOtherMethod(string fourth, bool fifth, int sixth)", SemVerChangeType.Breaking)]
        [InlineData("MyMethod<T>(T first, bool second, int third)", "MyOtherMethod<T>(T first, bool second, int third)", SemVerChangeType.Breaking)]
        public async Task EvaluatesChangeOfName(
            string oldSignature,
            string newSignature,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new(NoParameters.Replace("public string MyMethod()", "public string " + oldSignature))
            };
            var newCode = new List<CodeSource>
            {
                new(NoParameters.Replace("public string MyMethod()", "public string " + newSignature))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(expected);
        }

        private static string NoParameters => @"
namespace MyNamespace 
{
    using System;

    public class MyClass
    {
        public string MyMethod()
        {
            return Guid.NewGuid().ToString();
        }
    }  
}
";
    }
}