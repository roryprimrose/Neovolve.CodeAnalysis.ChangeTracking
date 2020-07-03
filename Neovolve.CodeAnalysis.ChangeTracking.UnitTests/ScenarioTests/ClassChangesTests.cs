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

    public class ClassChangesTests
    {
        private readonly IChangeCalculator _calculator;
        private readonly ITestOutputHelper _output;

        public ClassChangesTests(ITestOutputHelper output)
        {
            _output = output;

            var logger = output.BuildLogger(LogLevel.Information);

            _calculator = ChangeCalculatorFactory.BuildCalculator(logger);
        }

        [Theory]
        [ClassData(typeof(TypeAccessModifierDataSet))]
        public async Task EvaluatesChangeOfClassAccessModifiers(
            string oldModifiers,
            string newModifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleClass.Replace("public class MyClass", oldModifiers + " class MyClass"))
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleClass.Replace("public class MyClass", newModifiers + " class MyClass"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [InlineData("", "", SemVerChangeType.None)]
        [InlineData("sealed", "sealed", SemVerChangeType.None)]
        [InlineData("static", "static", SemVerChangeType.None)]
        [InlineData("abstract", "abstract", SemVerChangeType.None)]
        [InlineData("sealed", "", SemVerChangeType.Feature)]
        [InlineData("static", "", SemVerChangeType.Breaking)]
        [InlineData("abstract", "", SemVerChangeType.Feature)]
        [InlineData("", "sealed", SemVerChangeType.Breaking)]
        [InlineData("", "static", SemVerChangeType.Breaking)]
        [InlineData("", "abstract", SemVerChangeType.Breaking)]
        public async Task EvaluatesChangeOfClassModifiers(
            string oldModifiers,
            string newModifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleClass.Replace("class MyClass", oldModifiers + " class MyClass"))
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleClass.Replace("class MyClass", newModifiers + " class MyClass"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Fact]
        public async Task ReturnsBreakingWhenClassChangesName()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleClass)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleClass.Replace("MyClass", "MyNewClass"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenClassChangesNamespace()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleClass)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleClass.Replace("MyNamespace", "MyNewNamespace"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenClassRemoved()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleClass)
            };
            var newCode = Array.Empty<CodeSource>();

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsFeatureWhenClassAdded()
        {
            var oldCode = Array.Empty<CodeSource>();
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleClass)
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public async Task ReturnsNoneWhenMatchingSameClass()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleClass)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleClass)
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Fact]
        public async Task ReturnsNoneWhenRenamingGenericTypeParameter()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(TypeDefinitionCode.ClassWithMultipleGenericConstraints)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(
                    TypeDefinitionCode.ClassWithMultipleGenericConstraints.Replace("TValue", "TUpdatedValue"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Fact(Skip = "Not implemented yet")]
        public void TestChildClassesAttributes()
        {
        }

        [Fact]
        public void TestChildInterfacesAttributes()
        {
        }

        [Fact(Skip = "Not implemented yet")]
        public void TestClassAttributes()
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
                _output.WriteLine(comparisonResult.Message);
            }
        }

        public string SingleClass =>
            @"
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