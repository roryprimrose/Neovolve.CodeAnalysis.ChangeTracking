using System;
using System.Collections.Generic;
using System.Text;

namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ScenarioTests
{
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
        [InlineData("","", SemVerChangeType.None)]
        [InlineData("","sealed override", SemVerChangeType.Breaking)]
        [InlineData("","virtual", SemVerChangeType.Feature)]
        [InlineData("","static", SemVerChangeType.Breaking)]
        [InlineData("","abstract", SemVerChangeType.Breaking)]
        [InlineData("","override", SemVerChangeType.None)]
        [InlineData("sealed override","", SemVerChangeType.Feature)]
        [InlineData("sealed override","sealed override", SemVerChangeType.None)]
        [InlineData("sealed override","virtual", SemVerChangeType.Feature)]
        [InlineData("sealed override","static", SemVerChangeType.Breaking)]
        [InlineData("sealed override","abstract", SemVerChangeType.Breaking)]
        [InlineData("sealed override","override", SemVerChangeType.Feature)]
        [InlineData("virtual","", SemVerChangeType.None)]
        [InlineData("virtual","sealed override", SemVerChangeType.Breaking)]
        [InlineData("virtual","virtual", SemVerChangeType.None)]
        [InlineData("virtual","static", SemVerChangeType.Breaking)]
        [InlineData("virtual","abstract", SemVerChangeType.Breaking)]
        [InlineData("virtual","override", SemVerChangeType.None)]
        [InlineData("static","", SemVerChangeType.Breaking)]
        [InlineData("static","sealed override", SemVerChangeType.Breaking)]
        [InlineData("static","virtual", SemVerChangeType.Breaking)]
        [InlineData("static","static", SemVerChangeType.None)]
        [InlineData("static","abstract", SemVerChangeType.Breaking)]
        [InlineData("static","override", SemVerChangeType.Breaking)]
        [InlineData("abstract","", SemVerChangeType.Breaking)]
        [InlineData("abstract","sealed override", SemVerChangeType.Breaking)]
        [InlineData("abstract","virtual", SemVerChangeType.None)]
        [InlineData("abstract","static", SemVerChangeType.Breaking)]
        [InlineData("abstract","abstract", SemVerChangeType.None)]
        [InlineData("abstract","override", SemVerChangeType.Feature)]
        [InlineData("override","", SemVerChangeType.None)]
        [InlineData("override","sealed override", SemVerChangeType.Breaking)]
        [InlineData("override","virtual", SemVerChangeType.None)]
        [InlineData("override","static", SemVerChangeType.Breaking)]
        [InlineData("override","abstract", SemVerChangeType.Breaking)]
        [InlineData("override","override", SemVerChangeType.None)]
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

        [Fact]
        public void TestPropertyAttributes()
        {
            throw new NotImplementedException();
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
