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

    public class FieldChangesTests
    {
        private readonly IChangeCalculator _calculator;
        private readonly ITestOutputHelper _output;

        public FieldChangesTests(ITestOutputHelper output)
        {
            _output = output;

            var logger = output.BuildLogger(LogLevel.Information);

            _calculator = ChangeCalculatorFactory.BuildCalculator(logger);
        }

        [Fact]
        public async Task EvaluatesBreakingChangeWhenReturnTypeIsChangedGenericTypeParameter()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(FieldOnTypeWithMultipleGenericTypeParameters)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(
                    FieldOnTypeWithMultipleGenericTypeParameters.Replace("TKey MyField", "TValue MyField"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
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
                new CodeSource(SingleField.Replace("public string MyField", oldModifiers + " string MyField"))
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleField.Replace("public string MyField", newModifiers + " string MyField"))
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
                new CodeSource(SingleField.Replace("string MyField", oldModifiers + " string MyField"))
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleField.Replace("string MyField", newModifiers + " string MyField"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Fact]
        public async Task EvaluatesNoChangeWhenReturnTypeIsRenamedGenericTypeParameter()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(FieldOnTypeWithMultipleGenericTypeParameters)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(FieldOnTypeWithMultipleGenericTypeParameters.Replace("TKey", "TMyKey"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Fact]
        public async Task ReturnsBreakingWhenFieldChangesName()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleField)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleField.Replace("MyField", "MyNewField"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenFieldRemoved()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleField)
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
                new CodeSource(SingleField)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleField.Replace("string MyField", "bool MyField"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsFeatureWhenFieldAdded()
        {
            var oldCode = Array.Empty<CodeSource>();
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleField)
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public async Task ReturnsNoneWhenMatchingSameField()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleField)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleField)
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Fact(Skip = "Not implemented yet")]
        public void TestFieldAttributes()
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

        public string FieldOnTypeWithMultipleGenericTypeParameters => @"
namespace MyNamespace 
{
    [ClassAttribute(123, false, myName: ""on the class"")]
    public class MyClass<TKey, TValue>
    {
        [PropertyAttribute(344, true, myName: ""on the property"")]
        public TKey MyProperty { get; set; }

        [FieldAttribute(885, myName: ""on the field"")]
        public TKey MyField;
    }  
}
";

        public string SingleField => @"
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