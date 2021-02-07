namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;
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
        [ClassData(typeof(AccessModifiersDataSet))]
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

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [InlineData("", "", SemVerChangeType.None)]
        [InlineData("", "abstract", SemVerChangeType.Breaking)]
        [InlineData("", "partial", SemVerChangeType.None)]
        [InlineData("", "sealed", SemVerChangeType.Breaking)]
        [InlineData("", "static", SemVerChangeType.Breaking)]
        [InlineData("", "abstract partial", SemVerChangeType.Breaking)]
        [InlineData("", "static partial", SemVerChangeType.Breaking)]
        [InlineData("", "sealed partial", SemVerChangeType.Breaking)]
        [InlineData("", "partial abstract", SemVerChangeType.Breaking)]
        [InlineData("", "partial static", SemVerChangeType.Breaking)]
        [InlineData("", "partial sealed", SemVerChangeType.Breaking)]
        [InlineData("abstract", "", SemVerChangeType.Breaking)]
        [InlineData("abstract", "abstract", SemVerChangeType.None)]
        [InlineData("abstract", "partial", SemVerChangeType.Breaking)]
        [InlineData("abstract", "sealed", SemVerChangeType.Breaking)]
        [InlineData("abstract", "static", SemVerChangeType.Breaking)]
        [InlineData("abstract", "abstract partial", SemVerChangeType.None)]
        [InlineData("abstract", "static partial", SemVerChangeType.Breaking)]
        [InlineData("abstract", "sealed partial", SemVerChangeType.Breaking)]
        [InlineData("abstract", "partial abstract", SemVerChangeType.None)]
        [InlineData("abstract", "partial static", SemVerChangeType.Breaking)]
        [InlineData("abstract", "partial sealed", SemVerChangeType.Breaking)]
        [InlineData("partial", "", SemVerChangeType.None)]
        [InlineData("partial", "abstract", SemVerChangeType.Breaking)]
        [InlineData("partial", "partial", SemVerChangeType.None)]
        [InlineData("partial", "sealed", SemVerChangeType.Breaking)]
        [InlineData("partial", "static", SemVerChangeType.Breaking)]
        [InlineData("partial", "abstract partial", SemVerChangeType.Breaking)]
        [InlineData("partial", "static partial", SemVerChangeType.Breaking)]
        [InlineData("partial", "sealed partial", SemVerChangeType.Breaking)]
        [InlineData("partial", "partial abstract", SemVerChangeType.Breaking)]
        [InlineData("partial", "partial static", SemVerChangeType.Breaking)]
        [InlineData("partial", "partial sealed", SemVerChangeType.Breaking)]
        [InlineData("sealed", "", SemVerChangeType.Feature)]
        [InlineData("sealed", "abstract", SemVerChangeType.Breaking)]
        [InlineData("sealed", "partial", SemVerChangeType.Feature)]
        [InlineData("sealed", "sealed", SemVerChangeType.None)]
        [InlineData("sealed", "static", SemVerChangeType.Breaking)]
        [InlineData("sealed", "abstract partial", SemVerChangeType.Breaking)]
        [InlineData("sealed", "static partial", SemVerChangeType.Breaking)]
        [InlineData("sealed", "sealed partial", SemVerChangeType.None)]
        [InlineData("sealed", "partial abstract", SemVerChangeType.Breaking)]
        [InlineData("sealed", "partial static", SemVerChangeType.Breaking)]
        [InlineData("sealed", "partial sealed", SemVerChangeType.None)]
        [InlineData("static", "", SemVerChangeType.Breaking)]
        [InlineData("static", "abstract", SemVerChangeType.Breaking)]
        [InlineData("static", "partial", SemVerChangeType.Breaking)]
        [InlineData("static", "sealed", SemVerChangeType.Breaking)]
        [InlineData("static", "static", SemVerChangeType.None)]
        [InlineData("static", "abstract partial", SemVerChangeType.Breaking)]
        [InlineData("static", "static partial", SemVerChangeType.None)]
        [InlineData("static", "sealed partial", SemVerChangeType.Breaking)]
        [InlineData("static", "partial abstract", SemVerChangeType.Breaking)]
        [InlineData("static", "partial static", SemVerChangeType.None)]
        [InlineData("static", "partial sealed", SemVerChangeType.Breaking)]
        [InlineData("abstract partial", "", SemVerChangeType.Breaking)]
        [InlineData("abstract partial", "abstract", SemVerChangeType.None)]
        [InlineData("abstract partial", "partial", SemVerChangeType.Breaking)]
        [InlineData("abstract partial", "sealed", SemVerChangeType.Breaking)]
        [InlineData("abstract partial", "static", SemVerChangeType.Breaking)]
        [InlineData("abstract partial", "abstract partial", SemVerChangeType.None)]
        [InlineData("abstract partial", "static partial", SemVerChangeType.Breaking)]
        [InlineData("abstract partial", "sealed partial", SemVerChangeType.Breaking)]
        [InlineData("abstract partial", "partial abstract", SemVerChangeType.None)]
        [InlineData("abstract partial", "partial static", SemVerChangeType.Breaking)]
        [InlineData("abstract partial", "partial sealed", SemVerChangeType.Breaking)]
        [InlineData("static partial", "", SemVerChangeType.Breaking)]
        [InlineData("static partial", "abstract", SemVerChangeType.Breaking)]
        [InlineData("static partial", "partial", SemVerChangeType.Breaking)]
        [InlineData("static partial", "sealed", SemVerChangeType.Breaking)]
        [InlineData("static partial", "static", SemVerChangeType.None)]
        [InlineData("static partial", "abstract partial", SemVerChangeType.Breaking)]
        [InlineData("static partial", "static partial", SemVerChangeType.None)]
        [InlineData("static partial", "sealed partial", SemVerChangeType.Breaking)]
        [InlineData("static partial", "partial abstract", SemVerChangeType.Breaking)]
        [InlineData("static partial", "partial static", SemVerChangeType.None)]
        [InlineData("static partial", "partial sealed", SemVerChangeType.Breaking)]
        [InlineData("sealed partial", "", SemVerChangeType.Feature)]
        [InlineData("sealed partial", "abstract", SemVerChangeType.Breaking)]
        [InlineData("sealed partial", "partial", SemVerChangeType.Feature)]
        [InlineData("sealed partial", "sealed", SemVerChangeType.None)]
        [InlineData("sealed partial", "static", SemVerChangeType.Breaking)]
        [InlineData("sealed partial", "abstract partial", SemVerChangeType.Breaking)]
        [InlineData("sealed partial", "static partial", SemVerChangeType.Breaking)]
        [InlineData("sealed partial", "sealed partial", SemVerChangeType.None)]
        [InlineData("sealed partial", "partial abstract", SemVerChangeType.Breaking)]
        [InlineData("sealed partial", "partial static", SemVerChangeType.Breaking)]
        [InlineData("sealed partial", "partial sealed", SemVerChangeType.None)]
        [InlineData("partial abstract", "", SemVerChangeType.Breaking)]
        [InlineData("partial abstract", "abstract", SemVerChangeType.None)]
        [InlineData("partial abstract", "partial", SemVerChangeType.Breaking)]
        [InlineData("partial abstract", "sealed", SemVerChangeType.Breaking)]
        [InlineData("partial abstract", "static", SemVerChangeType.Breaking)]
        [InlineData("partial abstract", "abstract partial", SemVerChangeType.None)]
        [InlineData("partial abstract", "static partial", SemVerChangeType.Breaking)]
        [InlineData("partial abstract", "sealed partial", SemVerChangeType.Breaking)]
        [InlineData("partial abstract", "partial abstract", SemVerChangeType.None)]
        [InlineData("partial abstract", "partial static", SemVerChangeType.Breaking)]
        [InlineData("partial abstract", "partial sealed", SemVerChangeType.Breaking)]
        [InlineData("partial static", "", SemVerChangeType.Breaking)]
        [InlineData("partial static", "abstract", SemVerChangeType.Breaking)]
        [InlineData("partial static", "partial", SemVerChangeType.Breaking)]
        [InlineData("partial static", "sealed", SemVerChangeType.Breaking)]
        [InlineData("partial static", "static", SemVerChangeType.None)]
        [InlineData("partial static", "abstract partial", SemVerChangeType.Breaking)]
        [InlineData("partial static", "static partial", SemVerChangeType.None)]
        [InlineData("partial static", "sealed partial", SemVerChangeType.Breaking)]
        [InlineData("partial static", "partial abstract", SemVerChangeType.Breaking)]
        [InlineData("partial static", "partial static", SemVerChangeType.None)]
        [InlineData("partial static", "partial sealed", SemVerChangeType.Breaking)]
        [InlineData("partial sealed", "", SemVerChangeType.Feature)]
        [InlineData("partial sealed", "abstract", SemVerChangeType.Breaking)]
        [InlineData("partial sealed", "partial", SemVerChangeType.Feature)]
        [InlineData("partial sealed", "sealed", SemVerChangeType.None)]
        [InlineData("partial sealed", "static", SemVerChangeType.Breaking)]
        [InlineData("partial sealed", "abstract partial", SemVerChangeType.Breaking)]
        [InlineData("partial sealed", "static partial", SemVerChangeType.Breaking)]
        [InlineData("partial sealed", "sealed partial", SemVerChangeType.None)]
        [InlineData("partial sealed", "partial abstract", SemVerChangeType.Breaking)]
        [InlineData("partial sealed", "partial static", SemVerChangeType.Breaking)]
        [InlineData("partial sealed", "partial sealed", SemVerChangeType.None)]
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

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
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

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
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

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
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

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
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

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
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
                new CodeSource(TypeDefinitionCode.ClassWithMultipleGenericConstraints)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(
                    TypeDefinitionCode.ClassWithMultipleGenericConstraints.Replace("TValue", "TUpdatedValue"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Theory]
        [InlineData("[JsonPropertyName(\"item\")]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[JsonPropertyName(\"item\")]", AttributeCompareOption.ByExpression, SemVerChangeType.None)]
        [InlineData("[JsonPropertyName(\"item\")]", AttributeCompareOption.All, SemVerChangeType.None)]
        [InlineData("[JsonPropertyNameAttribute(\"item\")]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[JsonPropertyNameAttribute(\"item\")]", AttributeCompareOption.ByExpression,
            SemVerChangeType.None)]
        [InlineData("[JsonPropertyNameAttribute(\"item\")]", AttributeCompareOption.All, SemVerChangeType.None)]
        [InlineData("", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("", AttributeCompareOption.ByExpression, SemVerChangeType.Breaking)]
        [InlineData("", AttributeCompareOption.All, SemVerChangeType.Breaking)]
        [InlineData("[JsonPropertyName]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[JsonPropertyName]", AttributeCompareOption.ByExpression, SemVerChangeType.Breaking)]
        [InlineData("[JsonPropertyName]", AttributeCompareOption.All, SemVerChangeType.Breaking)]
        [InlineData("[JsonPropertyName(\"value\")]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[JsonPropertyName(\"value\")]", AttributeCompareOption.ByExpression, SemVerChangeType.Breaking)]
        [InlineData("[JsonPropertyName(\"value\")]", AttributeCompareOption.All, SemVerChangeType.Breaking)]
        [InlineData("[JsonPropertyName(name: \"value\")]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[JsonPropertyName(name: \"value\")]", AttributeCompareOption.ByExpression,
            SemVerChangeType.Breaking)]
        [InlineData("[JsonPropertyName(name: \"value\")]", AttributeCompareOption.All, SemVerChangeType.Breaking)]
        [InlineData("[SomethingElse]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[SomethingElse]", AttributeCompareOption.ByExpression, SemVerChangeType.Breaking)]
        [InlineData("[SomethingElse]", AttributeCompareOption.All, SemVerChangeType.Breaking)]
        public async Task ReturnsResultBasedOnAttributesOnChildClass(string updatedValue,
            AttributeCompareOption compareOption,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(TypeDefinitionCode.ChildClassWithAttribute)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(
                    TypeDefinitionCode.ChildClassWithAttribute.Replace("[JsonPropertyName(\"item\")]", updatedValue))
            };

            var options = OptionsFactory.BuildOptions().Set(x => x.CompareAttributes = compareOption);

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [InlineData("[JsonPropertyName(\"item\")]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[JsonPropertyName(\"item\")]", AttributeCompareOption.ByExpression, SemVerChangeType.None)]
        [InlineData("[JsonPropertyName(\"item\")]", AttributeCompareOption.All, SemVerChangeType.None)]
        [InlineData("[JsonPropertyNameAttribute(\"item\")]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[JsonPropertyNameAttribute(\"item\")]", AttributeCompareOption.ByExpression,
            SemVerChangeType.None)]
        [InlineData("[JsonPropertyNameAttribute(\"item\")]", AttributeCompareOption.All, SemVerChangeType.None)]
        [InlineData("", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("", AttributeCompareOption.ByExpression, SemVerChangeType.Breaking)]
        [InlineData("", AttributeCompareOption.All, SemVerChangeType.Breaking)]
        [InlineData("[JsonPropertyName]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[JsonPropertyName]", AttributeCompareOption.ByExpression, SemVerChangeType.Breaking)]
        [InlineData("[JsonPropertyName]", AttributeCompareOption.All, SemVerChangeType.Breaking)]
        [InlineData("[JsonPropertyName(\"value\")]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[JsonPropertyName(\"value\")]", AttributeCompareOption.ByExpression, SemVerChangeType.Breaking)]
        [InlineData("[JsonPropertyName(\"value\")]", AttributeCompareOption.All, SemVerChangeType.Breaking)]
        [InlineData("[JsonPropertyName(name: \"value\")]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[JsonPropertyName(name: \"value\")]", AttributeCompareOption.ByExpression,
            SemVerChangeType.Breaking)]
        [InlineData("[JsonPropertyName(name: \"value\")]", AttributeCompareOption.All, SemVerChangeType.Breaking)]
        [InlineData("[SomethingElse]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[SomethingElse]", AttributeCompareOption.ByExpression, SemVerChangeType.Breaking)]
        [InlineData("[SomethingElse]", AttributeCompareOption.All, SemVerChangeType.Breaking)]
        public async Task ReturnsResultBasedOnAttributesOnClass(string updatedValue,
            AttributeCompareOption compareOption,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(TypeDefinitionCode.ClassWithAttribute)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(
                    TypeDefinitionCode.ClassWithAttribute.Replace("[JsonPropertyName(\"item\")]", updatedValue))
            };

            var options = OptionsFactory.BuildOptions().Set(x => x.CompareAttributes = compareOption);

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
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