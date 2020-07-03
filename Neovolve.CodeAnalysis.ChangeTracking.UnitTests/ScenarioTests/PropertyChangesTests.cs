namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Xunit;
    using Xunit.Abstractions;

    public class PropertyChangesTests
    {
        private readonly IChangeCalculator _calculator;
        private readonly ITestOutputHelper _output;

        public PropertyChangesTests(ITestOutputHelper output)
        {
            _output = output;

            var logger = output.BuildLogger(LogLevel.Information);

            _calculator = ChangeCalculatorFactory.BuildCalculator(logger);
        }

        [Theory]
        [ClassData(typeof(PropertyModifierDataSet))]
        public async Task EvaluatesChangeOfPropertyModifiers(string oldModifiers, string newModifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("string MyProperty", oldModifiers + " string MyProperty"))
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("string MyProperty", newModifiers + " string MyProperty"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [ClassData(typeof(ScopeChangeDataSet))]
        public async Task EvaluatesChangeOfPropertyScope(string oldScope, string newScope, SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("public string MyProperty", oldScope + " string MyProperty"))
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("public string MyProperty", newScope + " string MyProperty"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Fact]
        public async Task ReturnsBreakingWhenPropertyChangesName()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("MyProperty", "MyNewProperty"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenPropertyRemoved()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty)
            };
            var newCode = Array.Empty<CodeSource>();

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsFeatureWhenPropertyAdded()
        {
            var oldCode = Array.Empty<CodeSource>();
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty)
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public async Task ReturnsNoneWhenMatchingSameProperty()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty)
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Fact(Skip = "Not implemented yet")]
        public void TestPropertyAttributes()
        {
        }

        private void OutputResult(ChangeCalculatorResult result)
        {
            _output.WriteLine($"Overall result: {result.ChangeType}");
            _output.WriteLine(string.Empty);

            foreach (var comparisonResult in result.ComparisonResults)
            {
                _output.WriteLine(comparisonResult.Message);
            }
        }

        public string SingleProperty => @"
namespace MyNamespace 
{
    [ClassAttribute(123, false, myName: ""on the class"")]
    public class MyClass
    {
        [PropertyAttribute(344, true, myName: ""on the property"")]
        public string MyProperty { get; set; }

        [FieldAttribute(885, myName: ""on the field"")]
        public string MyField;
    }  
}
";
    }
}