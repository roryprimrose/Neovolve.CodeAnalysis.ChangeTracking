namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class PropertyAccessorMatchEvaluatorTests
    {
        [Fact]
        public void MatchItemsIdentifiesPropertyAccessorsNotMatching()
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

            var results = sut.MatchItems(oldPropertyAccessors, newPropertyAccessors);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldMatchingPropertyAccessor);
            results.MatchingItems.First().NewItem.Should().Be(newMatchingPropertyAccessor);
            results.ItemsAdded.Should().HaveCount(1);
            results.ItemsAdded.First().Should().Be(newPropertyAccessor);
            results.ItemsRemoved.Should().HaveCount(1);
            results.ItemsRemoved.First().Should().Be(oldPropertyAccessor);
        }

        [Fact]
        public void MatchItemsReturnsSinglePropertyAccessorMatchingByName()
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

            var results = sut.MatchItems(oldPropertyAccessors, newPropertyAccessors);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldPropertyAccessor);
            results.MatchingItems.First().NewItem.Should().Be(newPropertyAccessor);
            results.ItemsAdded.Should().BeEmpty();
            results.ItemsRemoved.Should().BeEmpty();
        }

        [Fact]
        public void MatchItemsThrowsExceptionWithNullNewItems()
        {
            var oldItems = Array.Empty<PropertyAccessorDefinition>();

            var sut = new PropertyAccessorMatchEvaluator();

            Action action = () => sut.MatchItems(oldItems, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MatchItemsThrowsExceptionWithNullOldItems()
        {
            var newItems = Array.Empty<PropertyAccessorDefinition>();

            var sut = new PropertyAccessorMatchEvaluator();

            Action action = () => sut.MatchItems(null!, newItems);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}