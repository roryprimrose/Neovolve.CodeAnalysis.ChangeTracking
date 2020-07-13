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
        [InlineData("", SemVerChangeType.Feature)]
        [InlineData("internal", SemVerChangeType.None)]
        [InlineData("private", SemVerChangeType.None)]
        [InlineData("protected", SemVerChangeType.Feature)]
        [InlineData("protected internal", SemVerChangeType.Feature)]
        public async Task EvaluatesAddingGetAccessorWithAccessModifiers(
            string modifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("get;", string.Empty))
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("get;", modifiers + " get;"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [InlineData("", SemVerChangeType.Feature)]
        [InlineData("internal", SemVerChangeType.None)]
        [InlineData("private", SemVerChangeType.None)]
        [InlineData("protected", SemVerChangeType.Feature)]
        [InlineData("protected internal", SemVerChangeType.Feature)]
        public async Task EvaluatesAddingSetAccessorWithAccessModifiers(
            string modifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("set;", string.Empty))
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("set;", modifiers + " set;"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [ClassData(typeof(AccessModifierDataSet))]
        public async Task EvaluatesChangeOfAccessModifiers(
            string oldModifiers,
            string newModifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("public string MyProperty", oldModifiers + " string MyProperty"))
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("public string MyProperty", newModifiers + " string MyProperty"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [ClassData(typeof(PropertyAccessorAccessModifierDataSet))]
        public async Task EvaluatesChangeOfGetAccessorAccessModifiers(
            string oldModifiers,
            string newModifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("get;", oldModifiers + " get;"))
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("get;", newModifiers + " get;"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [ClassData(typeof(MemberModifiersDataSet))]
        public async Task EvaluatesChangeOfModifiers(string oldModifiers, string newModifiers,
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
        [ClassData(typeof(PropertyAccessorAccessModifierDataSet))]
        public async Task EvaluatesChangeOfSetAccessorAccessModifiers(
            string oldModifiers,
            string newModifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("set;", oldModifiers + " set;"))
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("set;", newModifiers + " set;"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [InlineData("", SemVerChangeType.Breaking)]
        [InlineData("internal", SemVerChangeType.None)]
        [InlineData("private", SemVerChangeType.None)]
        [InlineData("protected", SemVerChangeType.Breaking)]
        [InlineData("protected internal", SemVerChangeType.Breaking)]
        public async Task EvaluatesRemovingGetAccessorWithAccessModifiers(
            string modifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("get;", modifiers + " get;"))
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("get;", string.Empty))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [InlineData("", SemVerChangeType.Breaking)]
        [InlineData("internal", SemVerChangeType.None)]
        [InlineData("private", SemVerChangeType.None)]
        [InlineData("protected", SemVerChangeType.Breaking)]
        [InlineData("protected internal", SemVerChangeType.Breaking)]
        public async Task EvaluatesRemovingSetAccessorWithAccessModifiers(
            string modifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("set;", modifiers + " set;"))
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("set;", string.Empty))
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
        public async Task ReturnsBreakingWhenReturnTypeChanged()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("string MyProperty", "bool MyProperty"))
            };

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
                _output.WriteLine(comparisonResult.ChangeType + ": " + comparisonResult.Message);
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