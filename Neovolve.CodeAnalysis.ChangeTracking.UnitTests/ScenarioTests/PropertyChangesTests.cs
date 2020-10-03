namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ScenarioTests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;
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

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
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

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Fact]
        public async Task EvaluatesBreakingChangeWhenReturnTypeIsChangedGenericTypeParameter()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(PropertyOnTypeWithMultipleGenericTypeParameters)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(
                    PropertyOnTypeWithMultipleGenericTypeParameters.Replace("TKey MyProperty", "TValue MyProperty"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task EvaluatesBreakingWhenPropertyChangesName()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("MyProperty", "MyNewProperty"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task EvaluatesBreakingWhenPropertyRemoved()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(NoProperty)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task EvaluatesBreakingWhenReturnTypeChanged()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("string MyProperty", "bool MyProperty"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
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
                new CodeSource(SingleProperty.Replace("public string MyProperty", oldModifiers + " string MyProperty"))
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("public string MyProperty", newModifiers + " string MyProperty"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
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

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
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

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
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

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [InlineData("[PropertyAttribute(344, true, myName: \"on the property\")]", AttributeCompareOption.Skip,
            SemVerChangeType.None)]
        [InlineData("[PropertyAttribute(344, true, myName: \"on the property\")]", AttributeCompareOption.ByExpression,
            SemVerChangeType.None)]
        [InlineData("[PropertyAttribute(344, true, myName: \"on the property\")]", AttributeCompareOption.All,
            SemVerChangeType.None)]
        [InlineData("[Property(344, true, myName: \"on the property\")]", AttributeCompareOption.Skip,
            SemVerChangeType.None)]
        [InlineData("[Property(344, true, myName: \"on the property\")]", AttributeCompareOption.ByExpression,
            SemVerChangeType.None)]
        [InlineData("[Property(344, true, myName: \"on the property\")]", AttributeCompareOption.All,
            SemVerChangeType.None)]
        [InlineData("[Property(344, myName: \"on the property\")]", AttributeCompareOption.Skip, SemVerChangeType.None)]
        [InlineData("[Property(344, myName: \"on the property\")]", AttributeCompareOption.ByExpression,
            SemVerChangeType.None)]
        [InlineData("[Property(344, myName: \"on the property\")]", AttributeCompareOption.All,
            SemVerChangeType.Breaking)]
        [InlineData("[Property(344, true, myName: \"on the property updated\")]", AttributeCompareOption.Skip,
            SemVerChangeType.None)]
        [InlineData("[Property(344, true, myName: \"on the property updated\")]", AttributeCompareOption.ByExpression,
            SemVerChangeType.None)]
        [InlineData("[Property(344, true, myName: \"on the property updated\")]", AttributeCompareOption.All,
            SemVerChangeType.Breaking)]
        [InlineData("[Property(344, 654, true, myName: \"on the property updated\")]", AttributeCompareOption.Skip,
            SemVerChangeType.None)]
        [InlineData("[Property(344, 654, true, myName: \"on the property updated\")]",
            AttributeCompareOption.ByExpression, SemVerChangeType.None)]
        [InlineData("[Property(344, 654, true, myName: \"on the property updated\")]", AttributeCompareOption.All,
            SemVerChangeType.Breaking)]
        [InlineData("[Property(344, true, otherName: \"on the property updated\")]", AttributeCompareOption.Skip,
            SemVerChangeType.None)]
        [InlineData("[Property(344, true, otherName: \"on the property updated\")]",
            AttributeCompareOption.ByExpression, SemVerChangeType.None)]
        [InlineData("[Property(344, true, otherName: \"on the property updated\")]", AttributeCompareOption.All,
            SemVerChangeType.Breaking)]
        public async Task EvaluatesChangesToAttribute(string updatedValue,
            AttributeCompareOption compareOption,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty.Replace("[PropertyAttribute(344, true, myName: \"on the property\")]",
                    updatedValue))
            };

            var options = OptionsFactory.BuildOptions().Set(x => x.CompareAttributes = compareOption);

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
        }

        [Fact]
        public async Task EvaluatesFeatureWhenPropertyAdded()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(NoProperty)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public async Task EvaluatesNoChangeWhenMatchingSameProperty()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleProperty)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Fact]
        public async Task EvaluatesNoChangeWhenReturnTypeIsRenamedGenericTypeParameter()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(PropertyOnTypeWithMultipleGenericTypeParameters)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(PropertyOnTypeWithMultipleGenericTypeParameters.Replace("TKey", "TMyKey"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.None);
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

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
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

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(expected);
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

        public string NoProperty => @"
namespace MyNamespace 
{
    [ClassAttribute(123, false, myName: ""on the class"")]
    public class MyClass
    {
        [FieldAttribute(885, myName: ""on the field"")]
        public string MyField;
    }  
}
";

        public string PropertyOnTypeWithMultipleGenericTypeParameters => @"
namespace MyNamespace 
{
    [ClassAttribute(123, false, myName: ""on the class"")]
    public class MyClass<TKey, TValue>
    {
        [PropertyAttribute(344, true, myName: ""on the property"")]
        public TKey MyProperty { get; set; }

        [FieldAttribute(885, myName: ""on the field"")]
        public string MyField;
    }  
}
";

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