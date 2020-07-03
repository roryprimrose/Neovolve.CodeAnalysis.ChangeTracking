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

    public class InterfaceChangesTests
    {
        private readonly IChangeCalculator _calculator;
        private readonly ITestOutputHelper _output;

        public InterfaceChangesTests(ITestOutputHelper output)
        {
            _output = output;

            var logger = output.BuildLogger(LogLevel.Information);

            _calculator = ChangeCalculatorFactory.BuildCalculator(logger);
        }

        [Theory]
        [ClassData(typeof(TypeAccessModifierDataSet))]
        public async Task EvaluatesChangeOfInterfaceAccessModifiers(
            string oldModifiers,
            string newModifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(
                    SingleInterface.Replace("public interface MyInterface", oldModifiers + " interface MyInterface"))
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(
                    SingleInterface.Replace("public interface MyInterface", newModifiers + " interface MyInterface"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Fact]
        public async Task ReturnsBreakingWhenInterfaceChangesName()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleInterface)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleInterface.Replace("MyInterface", "MyNewInterface"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenInterfaceChangesNamespace()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleInterface)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleInterface.Replace("MyNamespace", "MyNewNamespace"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenInterfaceRemoved()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleInterface)
            };
            var newCode = Array.Empty<CodeSource>();

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsFeatureWhenInterfaceAdded()
        {
            var oldCode = Array.Empty<CodeSource>();
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleInterface)
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public async Task ReturnsNoneWhenMatchingSameInterface()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleInterface)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleInterface)
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.None);
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

        public string SingleInterface =>
            @"
namespace MyNamespace 
{
    [InterfaceAttribute(123, false, myName: ""on the class"")]
    public interface MyInterface
    {
        [PropertyAttribute(344, true, myName: ""on the property"")]
        string MyProperty { get; set; }

        [FieldAttribute(885, myName: ""on the field"")]
        string MyField;
    }  
}
";
    }
}