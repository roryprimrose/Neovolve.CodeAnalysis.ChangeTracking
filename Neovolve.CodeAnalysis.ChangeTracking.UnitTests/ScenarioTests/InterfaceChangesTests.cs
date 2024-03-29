﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ScenarioTests
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

    public class InterfaceChangesTests
    {
        public const string EmptyInterface =
            @"
namespace MyNamespace 
{
    [InterfaceAttribute(123, false, myName: ""on the interface"")]
    public interface MyInterface
    {
    }  
}
";

        public const string InterfaceWithDefaultMethod =
            @"
namespace MyNamespace 
{
    [InterfaceAttribute(123, false, myName: ""on the interface"")]
    public interface MyInterface
    {
        void DoSomething() { System.Diagnostics.Debug.WriteLine(""IA.M""); }
    }  
}
";

        public const string InterfaceWithMethod =
            @"
namespace MyNamespace 
{
    [InterfaceAttribute(123, false, myName: ""on the interface"")]
    public interface MyInterface
    {
        [MethodAttribute(23, false, myName: ""on the method"")]
        DateTime MyMethod(int first, bool second, string third);
    }  
}
";

        public const string InterfaceWithProperty =
            @"
namespace MyNamespace 
{
    [InterfaceAttribute(123, false, myName: ""on the interface"")]
    public interface MyInterface
    {
        [PropertyAttribute(344, true, myName: ""on the property"")]
        string MyProperty { get; set; }
    }  
}
";

        public const string SingleInterface =
            @"
namespace MyNamespace 
{
    [InterfaceAttribute(123, false, myName: ""on the interface"")]
    public interface MyInterface
    {
        [PropertyAttribute(344, true, myName: ""on the property"")]
        string MyProperty { get; set; }

        [MethodAttribute(23, false, myName: ""on the method"")]
        DateTime MyMethod(int first, bool second, string third);
    }  
}
";

        private readonly IChangeCalculator _calculator;

        public InterfaceChangesTests(ITestOutputHelper output)
        {
            var logger = output.BuildLogger(LogLevel.Information);

            _calculator = ChangeCalculatorFactory.BuildCalculator(logger);
        }

        [Theory]
        [ClassData(typeof(AccessModifiersDataSet))]
        public async Task EvaluatesChangeOfInterfaceAccessModifiers(
            string oldModifiers,
            string newModifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new(
                    SingleInterface.Replace("public interface MyInterface", oldModifiers + " interface MyInterface"))
            };
            var newCode = new List<CodeSource>
            {
                new(
                    SingleInterface.Replace("public interface MyInterface", newModifiers + " interface MyInterface"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(expected);
        }

        [Theory]
        [InlineData(InterfaceWithMethod, SemVerChangeType.Breaking)]
        [InlineData(InterfaceWithDefaultMethod, SemVerChangeType.Feature)]
        public async Task ReturnsBreakingWhenInterfaceAddsMethod(string code, SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new(EmptyInterface)
            };
            var newCode = new List<CodeSource>
            {
                new(code)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(expected);
        }

        [Fact]
        public async Task ReturnsBreakingWhenInterfaceAddsProperty()
        {
            var oldCode = new List<CodeSource>
            {
                new(EmptyInterface)
            };
            var newCode = new List<CodeSource>
            {
                new(InterfaceWithProperty)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenInterfaceChangesName()
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleInterface)
            };
            var newCode = new List<CodeSource>
            {
                new(SingleInterface.Replace("MyInterface", "MyNewInterface"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenInterfaceChangesNamespace()
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleInterface)
            };
            var newCode = new List<CodeSource>
            {
                new(SingleInterface.Replace("MyNamespace", "MyNewNamespace"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenInterfaceRemoved()
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleInterface)
            };
            var newCode = Array.Empty<CodeSource>();

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsFeatureWhenInterfaceAdded()
        {
            var oldCode = Array.Empty<CodeSource>();
            var newCode = new List<CodeSource>
            {
                new(SingleInterface)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public async Task ReturnsNoneWhenMatchingSameInterface()
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleInterface)
            };
            var newCode = new List<CodeSource>
            {
                new(SingleInterface)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

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
        public async Task ReturnsResultBasedOnAttributesOnChildInterface(string updatedValue,
            AttributeCompareOption compareOption,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new(TypeDefinitionCode.ChildInterfaceWithAttribute)
            };
            var newCode = new List<CodeSource>
            {
                new(
                    TypeDefinitionCode.ChildInterfaceWithAttribute.Replace("[JsonPropertyName(\"item\")]",
                        updatedValue))
            };

            var options = OptionsFactory.BuildOptions().Set(x => x.CompareAttributes = compareOption);

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

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
        public async Task ReturnsResultBasedOnAttributesOnInterface(string updatedValue,
            AttributeCompareOption compareOption,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new(TypeDefinitionCode.InterfaceWithAttribute)
            };
            var newCode = new List<CodeSource>
            {
                new(
                    TypeDefinitionCode.InterfaceWithAttribute.Replace("[JsonPropertyName(\"item\")]",
                        updatedValue))
            };

            var options = OptionsFactory.BuildOptions().Set(x => x.CompareAttributes = compareOption);

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(expected);
        }
    }
}