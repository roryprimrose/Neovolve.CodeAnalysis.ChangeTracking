namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Evaluators
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;

    public class ParameterEvaluatorTests : Tests<ParameterEvaluator>
    {
        [Fact]
        public void FindMatchesIdentifiesPropertiesNotMatching()
        {
            var oldParameter = new TestParameterDefinition();
            var newParameter = new TestParameterDefinition().Set(x => x.DeclaredIndex = 1);
            var oldMatchingParameter = new TestParameterDefinition().Set(x => x.DeclaredIndex = 1);
            var oldProperties = new[]
            {
                oldParameter, oldMatchingParameter
            };
            var newMatchingParameter = oldMatchingParameter.JsonClone();
            var newProperties = new[]
            {
                newMatchingParameter, newParameter
            };

            var results = SUT.FindMatches(oldProperties, newProperties);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldMatchingParameter);
            results.MatchingItems.First().NewItem.Should().Be(newMatchingParameter);
            results.ItemsAdded.Should().HaveCount(1);
            results.ItemsAdded.First().Should().Be(newParameter);
            results.ItemsRemoved.Should().HaveCount(1);
            results.ItemsRemoved.First().Should().Be(oldParameter);
        }

        [Fact]
        public void FindMatchesReturnsSingleParameterMatchingByIndex()
        {
            var oldParameter = new TestParameterDefinition();
            var oldProperties = new[]
            {
                oldParameter
            };
            var newParameter = oldParameter.JsonClone().Set(x => x.Name = Guid.NewGuid().ToString());
            var newProperties = new[]
            {
                newParameter
            };

            var results = SUT.FindMatches(oldProperties, newProperties);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldParameter);
            results.MatchingItems.First().NewItem.Should().Be(newParameter);
            results.ItemsAdded.Should().BeEmpty();
            results.ItemsRemoved.Should().BeEmpty();
        }

        [Fact]
        public void FindMatchesReturnsSingleParameterMatchingByName()
        {
            var oldParameter = new TestParameterDefinition();
            var oldProperties = new[]
            {
                oldParameter
            };
            var newParameter = oldParameter.JsonClone();
            var newProperties = new[]
            {
                newParameter
            };

            var results = SUT.FindMatches(oldProperties, newProperties);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldParameter);
            results.MatchingItems.First().NewItem.Should().Be(newParameter);
            results.ItemsAdded.Should().BeEmpty();
            results.ItemsRemoved.Should().BeEmpty();
        }

        [Fact]
        public void FindMatchesThrowsExceptionWithNullNewItems()
        {
            var oldItems = Array.Empty<ParameterDefinition>();

            var sut = new ParameterEvaluator();

            Action action = () => sut.FindMatches(oldItems, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void FindMatchesThrowsExceptionWithNullOldItems()
        {
            var newItems = Array.Empty<ParameterDefinition>();

            var sut = new ParameterEvaluator();

            Action action = () => sut.FindMatches(null!, newItems);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}