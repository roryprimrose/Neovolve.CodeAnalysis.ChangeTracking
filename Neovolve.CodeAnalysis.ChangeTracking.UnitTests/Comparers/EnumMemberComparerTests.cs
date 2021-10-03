namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class EnumMemberComparerTests : Tests<EnumMemberComparer>
    {
        private readonly ITestOutputHelper _output;

        public EnumMemberComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenMemberChangesName()
        {
            var options = TestComparerOptions.Default;
            var oldItem = new TestEnumMemberDefinition();
            var newItem = oldItem.JsonClone().Set(x => x.Name = Guid.NewGuid().ToString());
            var match = new ItemMatch<IEnumMemberDefinition>(oldItem, newItem);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().NotBeEmpty();

            var result = actual.Single();

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
            result.OldItem.Should().Be(oldItem);
            result.NewItem.Should().Be(newItem);
            result.Message.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData("", "123")]
        [InlineData("123", "")]
        [InlineData("123", "456")]
        [InlineData("123", "First | Second")]
        [InlineData("First | Second", "123")]
        public void CompareMatchReturnsBreakingWhenMemberValueChanged(string oldValue, string newValue)
        {
            var options = TestComparerOptions.Default;
            var oldItem = new TestEnumMemberDefinition().Set(x => x.Value = oldValue);
            var newItem = oldItem.JsonClone().Set(x => x.Value = newValue);
            var match = new ItemMatch<IEnumMemberDefinition>(oldItem, newItem);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().NotBeEmpty();

            var result = actual.Single();

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
            result.OldItem.Should().Be(oldItem);
            result.NewItem.Should().Be(newItem);
            result.Message.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenMemberWithImplicitValueChangesIndex()
        {
            var options = TestComparerOptions.Default;
            var oldItem = new TestEnumMemberDefinition().Set(x => x.Value = string.Empty);
            var newItem = oldItem.JsonClone().Set(x => x.Index = oldItem.Index + 1);
            var match = new ItemMatch<IEnumMemberDefinition>(oldItem, newItem);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().NotBeEmpty();

            var result = actual.Single();

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
            result.OldItem.Should().Be(oldItem);
            result.NewItem.Should().Be(newItem);
            result.Message.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData("First | Second | Third", "First | Second | Third")]
        [InlineData("First | Second | Third", "First | Third | Second")]
        [InlineData("First | Second | Third", "First|Third|Second")]
        [InlineData("First | Second | Third", "First|Second|Third")]
        public void CompareMatchReturnsEmptyResultsWhenBitwiseWhitespaceOrOrderingChanges(string oldValue,
            string newValue)
        {
            var options = TestComparerOptions.Default;
            var oldItem = new TestEnumMemberDefinition().Set(x => x.Value = oldValue);
            var newItem = oldItem.JsonClone().Set(x => x.Value = newValue);
            var match = new ItemMatch<IEnumMemberDefinition>(oldItem, newItem);

            var actual = SUT.CompareMatch(match, options);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchReturnsEmptyResultsWhenItemsMatch()
        {
            var options = TestComparerOptions.Default;
            var oldItem = new TestEnumMemberDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IEnumMemberDefinition>(oldItem, newItem);

            var actual = SUT.CompareMatch(match, options);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchReturnsResultsFromAttributeMatch()
        {
            var options = TestComparerOptions.Default;
            var oldItem = new TestEnumMemberDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IEnumMemberDefinition>(oldItem, newItem);
            var result = new ComparisonResult(SemVerChangeType.Breaking, new TestAttributeDefinition(),
                new TestAttributeDefinition(), Guid.NewGuid().ToString());
            var results = new List<ComparisonResult> { result };

            Service<IAttributeMatchProcessor>().CalculateChanges(oldItem.Attributes, newItem.Attributes, options)
                .Returns(results);

            var actual = SUT.CompareMatch(match, options);

            actual.Should().BeEquivalentTo(results);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullAttributeProcessor()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new EnumMemberComparer(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}