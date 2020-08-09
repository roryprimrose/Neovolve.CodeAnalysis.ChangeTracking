namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models;
    using Xunit;
    using Xunit.Abstractions;

    public class StructChangesTests
    {
        private readonly IChangeCalculator _calculator;
        private readonly ITestOutputHelper _output;

        public StructChangesTests(ITestOutputHelper output)
        {
            _output = output;

            var logger = output.BuildLogger(LogLevel.Information);

            _calculator = ChangeCalculatorFactory.BuildCalculator(logger);
        }

        [Theory]
        [ClassData(typeof(AccessModifierDataSet))]
        public async Task EvaluatesChangeOfStructAccessModifiers(
            string oldModifiers,
            string newModifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleStruct.Replace("public struct MyStruct", oldModifiers + " struct MyStruct"))
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleStruct.Replace("public struct MyStruct", newModifiers + " struct MyStruct"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [InlineData("", "", SemVerChangeType.None)]
        [InlineData("", "readonly", SemVerChangeType.Breaking)]
        [InlineData("", "partial", SemVerChangeType.None)]
        [InlineData("", "readonly partial", SemVerChangeType.Breaking)]
        [InlineData("readonly", "", SemVerChangeType.Feature)]
        [InlineData("readonly", "readonly", SemVerChangeType.None)]
        [InlineData("readonly", "partial", SemVerChangeType.Feature)]
        [InlineData("readonly", "readonly partial", SemVerChangeType.None)]
        [InlineData("partial", "", SemVerChangeType.None)]
        [InlineData("partial", "readonly", SemVerChangeType.Breaking)]
        [InlineData("partial", "partial", SemVerChangeType.None)]
        [InlineData("partial", "readonly partial", SemVerChangeType.Breaking)]
        [InlineData("readonly partial", "", SemVerChangeType.Feature)]
        [InlineData("readonly partial", "readonly", SemVerChangeType.None)]
        [InlineData("readonly partial", "partial", SemVerChangeType.Feature)]
        [InlineData("readonly partial", "readonly partial", SemVerChangeType.None)]
        public async Task EvaluatesChangeOfStructModifiers(
            string oldModifiers,
            string newModifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleStruct.Replace("struct MyStruct", oldModifiers + " struct MyStruct"))
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleStruct.Replace("struct MyStruct", newModifiers + " struct MyStruct"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Fact]
        public async Task ReturnsBreakingWhenStructChangesName()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleStruct)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleStruct.Replace("MyStruct", "MyNewStruct"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenStructChangesNamespace()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleStruct)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleStruct.Replace("MyNamespace", "MyNewNamespace"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenStructRemoved()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleStruct)
            };
            var newCode = Array.Empty<CodeSource>();

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsFeatureWhenStructAdded()
        {
            var oldCode = Array.Empty<CodeSource>();
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleStruct)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public async Task ReturnsNoneWhenMatchingSameStruct()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleStruct)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleStruct)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Fact]
        public async Task ReturnsNoneWhenRenamingGenericTypeParameter()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(TypeDefinitionCode.StructWithMultipleGenericConstraints)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(
                    TypeDefinitionCode.StructWithMultipleGenericConstraints.Replace("TValue", "TUpdatedValue"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Fact(Skip = "Not implemented yet")]
        public void TestChildStructsAttributes()
        {
        }

        [Fact]
        public void TestChildInterfacesAttributes()
        {
        }

        [Fact(Skip = "Not implemented yet")]
        public void TestStructAttributes()
        {
        }

        [Fact(Skip = "Not implemented yet")]
        public void TestGenericTypeConstraints()
        {
        }

        [Fact(Skip = "Not implemented yet")]
        public void TestGenericTypes()
        {
        }

        private void OutputResult(ChangeCalculatorResult result)
        {
            _output.WriteLine($"Overall result: {result.ChangeType}");
            _output.WriteLine(string.Empty);

            foreach (var comparisonResult in result.ComparisonResults)
            {
                _output.WriteLine(comparisonResult.ChangeType + ": " + comparisonResult.Message);
            }
        }

        public string SingleStruct =>
            @"
namespace MyNamespace 
{
    [StructAttribute(123, false, myName: ""on the struct"")]
    public struct MyStruct
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