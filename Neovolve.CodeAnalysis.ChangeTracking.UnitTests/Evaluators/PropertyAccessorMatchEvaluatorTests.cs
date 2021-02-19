namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Evaluators
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class PropertyAccessorMatchEvaluatorTests
    {
        [Fact]
        public void FindMatchesIdentifiesPropertyAccessorsNotMatching()
        {
            var executeStrategy = Model.UsingModule<ConfigurationModule>()
                .Ignoring<TestPropertyAccessorDefinition>(x => x.Attributes);
            var oldPropertyAccessor = executeStrategy.Create<TestPropertyAccessorDefinition>();
            var newPropertyAccessor = executeStrategy.Create<TestPropertyAccessorDefinition>();
            var oldMatchingPropertyAccessor = executeStrategy.Create<TestPropertyAccessorDefinition>();
            var oldPropertyAccessors = new[]
            {
                oldPropertyAccessor, oldMatchingPropertyAccessor
            };
            var newMatchingPropertyAccessor = executeStrategy.Create<TestPropertyAccessorDefinition>()
                .Set(x => x.Name = oldMatchingPropertyAccessor.Name);
            var newPropertyAccessors = new[]
            {
                newMatchingPropertyAccessor, newPropertyAccessor
            };

            var sut = new PropertyAccessorMatchEvaluator();

            var results = sut.FindMatches(oldPropertyAccessors, newPropertyAccessors);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldMatchingPropertyAccessor);
            results.MatchingItems.First().NewItem.Should().Be(newMatchingPropertyAccessor);
            results.ItemsAdded.Should().HaveCount(1);
            results.ItemsAdded.First().Should().Be(newPropertyAccessor);
            results.ItemsRemoved.Should().HaveCount(1);
            results.ItemsRemoved.First().Should().Be(oldPropertyAccessor);
        }

        [Fact]
        public void FindMatchesReturnsSinglePropertyAccessorMatchingByName()
        {
            var executeStrategy = Model.UsingModule<ConfigurationModule>()
                .Ignoring<TestPropertyAccessorDefinition>(x => x.Attributes);
            var oldPropertyAccessor = executeStrategy.Create<TestPropertyAccessorDefinition>();
            var oldPropertyAccessors = new[]
            {
                oldPropertyAccessor
            };
            var newPropertyAccessor = executeStrategy.Create<TestPropertyAccessorDefinition>()
                .Set(x => x.Name = oldPropertyAccessor.Name);
            var newPropertyAccessors = new[]
            {
                newPropertyAccessor
            };

            var sut = new PropertyAccessorMatchEvaluator();

            var results = sut.FindMatches(oldPropertyAccessors, newPropertyAccessors);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldPropertyAccessor);
            results.MatchingItems.First().NewItem.Should().Be(newPropertyAccessor);
            results.ItemsAdded.Should().BeEmpty();
            results.ItemsRemoved.Should().BeEmpty();
        }

        [Fact]
        public void FindMatchesThrowsExceptionWithNullNewItems()
        {
            var oldItems = Array.Empty<PropertyAccessorDefinition>();

            var sut = new PropertyAccessorMatchEvaluator();

            Action action = () => sut.FindMatches(oldItems, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void FindMatchesThrowsExceptionWithNullOldItems()
        {
            var newItems = Array.Empty<PropertyAccessorDefinition>();

            var sut = new PropertyAccessorMatchEvaluator();

            Action action = () => sut.FindMatches(null!, newItems);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}