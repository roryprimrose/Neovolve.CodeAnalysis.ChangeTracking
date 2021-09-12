namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ScenarioTests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Xunit;
    using Xunit.Abstractions;

    public class EnumChangesTests
    {
        private const string EnumMembersWithExplicitValues = @"
namespace MyNamespace 
{
    public enum MyEnum
    {
        First = 123,
        Second = 234,
        Third = 345
    }   
}
";

        private const string EnumMembersWithFlagsValues = @"
namespace MyNamespace 
{
    [Flags]
    public enum MyEnum
    {
        First = 1,
        Second = 2,
        Third = 4,
        All = First | Second | Third
    }   
}
";

        private const string EnumMembersWithImplicitValues = @"
namespace MyNamespace 
{
    public enum MyEnum
    {
        First,
        Second,
        Third
    }   
}
";

        private readonly IChangeCalculator _calculator;

        public EnumChangesTests(ITestOutputHelper output)
        {
            var logger = output.BuildLogger(LogLevel.Information);

            _calculator = ChangeCalculatorFactory.BuildCalculator(logger);
        }

        [Theory]
        [ClassData(typeof(EnumAccessModifiersDataSet))]
        public async Task EvaluatesChangeOfEnumAccessModifiers(
            string oldModifiers,
            string newModifiers,
            SemVerChangeType expected)
        {
            var oldCode = new List<CodeSource>
            {
                new(EnumMembersWithExplicitValues.Replace("public enum MyEnum", oldModifiers + " enum MyEnum"))
            };
            var newCode = new List<CodeSource>
            {
                new(EnumMembersWithExplicitValues.Replace("public enum MyEnum", newModifiers + " enum MyEnum"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(expected);
        }

        [Fact]
        public async Task EvaluatesReturnsBreakingWhenEnumRemoved()
        {
            var oldCode = new List<CodeSource>
            {
                new(EnumMembersWithExplicitValues)
            };
            var newCode = new List<CodeSource>
            {
                new(string.Empty)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task EvaluatesReturnsBreakingWhenExplicitValueAdded()
        {
            var oldCode = new List<CodeSource>
            {
                new(EnumMembersWithImplicitValues)
            };
            var newCode = new List<CodeSource>
            {
                new(EnumMembersWithImplicitValues.Replace("First", "First = 123"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task EvaluatesReturnsBreakingWhenExplicitValueChanged()
        {
            var oldCode = new List<CodeSource>
            {
                new(EnumMembersWithExplicitValues)
            };
            var newCode = new List<CodeSource>
            {
                new(EnumMembersWithExplicitValues.Replace("First = 123", "First = 1233"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task EvaluatesReturnsBreakingWhenExplicitValueRemoved()
        {
            var oldCode = new List<CodeSource>
            {
                new(EnumMembersWithExplicitValues)
            };
            var newCode = new List<CodeSource>
            {
                new(EnumMembersWithExplicitValues.Replace("First = 123", "First"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task EvaluatesReturnsBreakingWhenMemberAdded()
        {
            var oldCode = new List<CodeSource>
            {
                new(EnumMembersWithExplicitValues)
            };
            var newCode = new List<CodeSource>
            {
                new(EnumMembersWithExplicitValues.Replace("Third = 345", "Third = 345, Forth = 456"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public async Task EvaluatesReturnsBreakingWhenMemberRemoved()
        {
            var oldCode = new List<CodeSource>
            {
                new(EnumMembersWithExplicitValues)
            };
            var newCode = new List<CodeSource>
            {
                new(EnumMembersWithExplicitValues.Replace("First = 123,", string.Empty))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task EvaluatesReturnsBreakingWhenNameChanged()
        {
            var oldCode = new List<CodeSource>
            {
                new(EnumMembersWithExplicitValues)
            };
            var newCode = new List<CodeSource>
            {
                new(EnumMembersWithExplicitValues.Replace("MyEnum", "ChangedEnum"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task EvaluatesReturnsBreakingWhenNamespaceChanged()
        {
            var oldCode = new List<CodeSource>
            {
                new(EnumMembersWithExplicitValues)
            };
            var newCode = new List<CodeSource>
            {
                new(EnumMembersWithExplicitValues.Replace("MyNamespace", "ChangedNamespace"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Theory]
        [InlineData("byte", "")]
        [InlineData("", "byte")]
        [InlineData("byte", "long")]
        public async Task EvaluatesReturnsBreakingWhenUnderlyingTypeChanges(string oldValue, string newValue)
        {
            var oldCode = new List<CodeSource>
            {
                new(EnumMembersWithFlagsValues.Replace("MyEnum", "MyEnum : " + oldValue))
            };
            var newCode = new List<CodeSource>
            {
                new(EnumMembersWithFlagsValues.Replace("MyEnum", "MyEnum : " + newValue))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task EvaluatesReturnsFeatureWhenEnumAdded()
        {
            var oldCode = new List<CodeSource>
            {
                new(string.Empty)
            };
            var newCode = new List<CodeSource>
            {
                new(EnumMembersWithExplicitValues)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public async Task EvaluatesReturnsNoneMatchingSameEnum()
        {
            var oldCode = new List<CodeSource>
            {
                new(EnumMembersWithExplicitValues)
            };
            var newCode = new List<CodeSource>
            {
                new(EnumMembersWithExplicitValues)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("", "[Flags]")]
        [InlineData("[Flags]", "")]
        [InlineData("[Flags]", "[Flags]")]
        public async Task EvaluatesReturnsNoneWhenFlagsAttributeAddedOrRemoved(string oldAttribute, string newAttribute)
        {
            var oldCode = new List<CodeSource>
            {
                new(EnumMembersWithFlagsValues.Replace("[Flags]", oldAttribute))
            };
            var newCode = new List<CodeSource>
            {
                new(EnumMembersWithFlagsValues.Replace("[Flags]", newAttribute))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Theory]
        [InlineData("First|Second|Third")]
        [InlineData("Third | First | Second")]
        [InlineData("Third|First|Second")]
        public async Task EvaluatesReturnsNoneWhenOnlyBitwiseValueChangesWhitespaceOrOrdering(string updatedValue)
        {
            var oldCode = new List<CodeSource>
            {
                new(EnumMembersWithFlagsValues)
            };
            var newCode = new List<CodeSource>
            {
                new(EnumMembersWithFlagsValues.Replace("First | Second | Third", updatedValue))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("", "int")]
        [InlineData("int", "")]
        [InlineData("int", "int")]
        public async Task EvaluatesReturnsNoneWhenUnderlyingTypeChangesBetweenImplicitAndExplicitDefault(
            string oldValue, string newValue)
        {
            var oldCode = new List<CodeSource>
            {
                new(EnumMembersWithFlagsValues.Replace("MyEnum", "MyEnum : " + oldValue))
            };
            var newCode = new List<CodeSource>
            {
                new(EnumMembersWithFlagsValues.Replace("MyEnum", "MyEnum : " + newValue))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }
    }
}