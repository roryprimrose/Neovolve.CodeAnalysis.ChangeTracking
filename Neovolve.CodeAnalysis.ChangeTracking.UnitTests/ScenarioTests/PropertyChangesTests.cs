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
        public const string SingleProperty = @"
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

        private readonly IChangeCalculator _calculator;

        public PropertyChangesTests(ITestOutputHelper output)
        {
            var logger = output.BuildLogger(LogLevel.Information);

            _calculator = ChangeCalculatorFactory.BuildCalculator(logger);
        }

        [Fact]
        public async Task ChangeOfInterfacePropertyTypeReturnsBreaking()
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleProperty)
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("string MyProperty", "int MyProperty"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Theory]
        [InlineData("get;", "get;", SemVerChangeType.None)]
        [InlineData("get;", "set;", SemVerChangeType.Breaking)]
        [InlineData("get;", "init;", SemVerChangeType.Breaking)]
        [InlineData("get;", "get; set;", SemVerChangeType.Feature)]
        [InlineData("get;", "get; init;", SemVerChangeType.Feature)]
        [InlineData("set;", "set;", SemVerChangeType.None)]
        [InlineData("set;", "get;", SemVerChangeType.Breaking)]
        [InlineData("set;", "init;", SemVerChangeType.Breaking)]
        [InlineData("set;", "get; set;", SemVerChangeType.Feature)]
        [InlineData("set;", "get; init;", SemVerChangeType.Breaking)]
        [InlineData("init;", "init;", SemVerChangeType.None)]
        [InlineData("init;", "get;", SemVerChangeType.Breaking)]
        [InlineData("init;", "set;", SemVerChangeType.Feature)]
        [InlineData("init;", "get; set;", SemVerChangeType.Feature)]
        [InlineData("init;", "get; init;", SemVerChangeType.Feature)]
        [InlineData("get; set;", "get; set;", SemVerChangeType.None)]
        [InlineData("get; set;", "get;", SemVerChangeType.Breaking)]
        [InlineData("get; set;", "set;", SemVerChangeType.Breaking)]
        [InlineData("get; set;", "init;", SemVerChangeType.Breaking)]
        [InlineData("get; set;", "get; init;", SemVerChangeType.Breaking)]
        [InlineData("get; init;", "get; init;", SemVerChangeType.None)]
        [InlineData("get; init;", "get;", SemVerChangeType.Breaking)]
        [InlineData("get; init;", "set;", SemVerChangeType.Breaking)]
        [InlineData("get; init;", "init;", SemVerChangeType.Breaking)]
        [InlineData("get; init;", "get; set;", SemVerChangeType.Feature)]
        public async Task EvaluatesAccessors(
            string oldAccessors,
            string newAccessors,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("get; set;", oldAccessors))
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("get; set;", newAccessors))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(expected);
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
                new(SingleProperty.Replace("get;", string.Empty))
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("get;", modifiers + " get;"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [InlineData("", SemVerChangeType.Feature)]
        [InlineData("internal", SemVerChangeType.None)]
        [InlineData("private", SemVerChangeType.None)]
        [InlineData("protected", SemVerChangeType.Feature)]
        [InlineData("protected internal", SemVerChangeType.Feature)]
        public async Task EvaluatesAddingInitAccessorWithAccessModifiers(
            string modifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("set;", string.Empty))
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("set;", modifiers + " init;"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

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
                new(SingleProperty.Replace("set;", string.Empty))
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("set;", modifiers + " set;"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(expected);
        }

        [Fact]
        public async Task EvaluatesBreakingChangeWhenReturnTypeIsChangedGenericTypeParameter()
        {
            var oldCode = new List<CodeSource>
            {
                new(PropertyOnTypeWithMultipleGenericTypeParameters)
            };
            var newCode = new List<CodeSource>
            {
                new(
                    PropertyOnTypeWithMultipleGenericTypeParameters.Replace("TKey MyProperty", "TValue MyProperty"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task EvaluatesBreakingWhenAbstractPropertyAdded()
        {
            var oldCode = new List<CodeSource>
            {
                new(NoProperty)
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("public string MyProperty", "public abstract string MyProperty"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task EvaluatesBreakingWhenPropertyChangesName()
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleProperty)
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("MyProperty", "MyNewProperty"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task EvaluatesBreakingWhenPropertyRemoved()
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleProperty)
            };
            var newCode = new List<CodeSource>
            {
                new(NoProperty)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task EvaluatesBreakingWhenReturnTypeChanged()
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleProperty)
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("string MyProperty", "bool MyProperty"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
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
                new(SingleProperty.Replace("public string MyProperty", oldModifiers + " string MyProperty"))
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("public string MyProperty", newModifiers + " string MyProperty"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

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
                new(SingleProperty.Replace("get;", oldModifiers + " get;"))
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("get;", newModifiers + " get;"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [ClassData(typeof(PropertyAccessorAccessModifierDataSet))]
        public async Task EvaluatesChangeOfInitAccessorAccessModifiers(
            string oldModifiers,
            string newModifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("set;", oldModifiers + " init;"))
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("set;", newModifiers + " init;"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [ClassData(typeof(PropertyModifiersDataSet))]
        public async Task EvaluatesChangeOfModifiers(string oldModifiers, string newModifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("string MyProperty", oldModifiers + " string MyProperty"))
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("string MyProperty", newModifiers + " string MyProperty"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

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
                new(SingleProperty.Replace("set;", oldModifiers + " set;"))
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("set;", newModifiers + " set;"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

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
                new(SingleProperty)
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("[PropertyAttribute(344, true, myName: \"on the property\")]",
                    updatedValue))
            };

            var options = OptionsFactory.BuildOptions().Set(x => x.CompareAttributes = compareOption);

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(expected);
        }

        [Fact]
        public async Task EvaluatesFeatureWhenPropertyAdded()
        {
            var oldCode = new List<CodeSource>
            {
                new(NoProperty)
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public async Task EvaluatesNoChangeWhenMatchingSameProperty()
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleProperty)
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Fact]
        public async Task EvaluatesNoChangeWhenReturnTypeIsRenamedGenericTypeParameter()
        {
            var oldCode = new List<CodeSource>
            {
                new(PropertyOnTypeWithMultipleGenericTypeParameters)
            };
            var newCode = new List<CodeSource>
            {
                new(PropertyOnTypeWithMultipleGenericTypeParameters.Replace("TKey", "TMyKey"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

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
                new(SingleProperty.Replace("get;", modifiers + " get;"))
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("get;", string.Empty))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [InlineData("", SemVerChangeType.Breaking)]
        [InlineData("internal", SemVerChangeType.None)]
        [InlineData("private", SemVerChangeType.None)]
        [InlineData("protected", SemVerChangeType.Breaking)]
        [InlineData("protected internal", SemVerChangeType.Breaking)]
        public async Task EvaluatesRemovingInitAccessorWithAccessModifiers(
            string modifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("set;", modifiers + " init;"))
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("set;", string.Empty))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

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
                new(SingleProperty.Replace("set;", modifiers + " set;"))
            };
            var newCode = new List<CodeSource>
            {
                new(SingleProperty.Replace("set;", string.Empty))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(expected);
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
    }
}