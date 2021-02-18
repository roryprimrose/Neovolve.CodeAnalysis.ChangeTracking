namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Evaluators
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class PropertyMatchEvaluatorTests
    {
        [Fact]
        public void MatchItemsIdentifiesPropertiesNotMatching()
        {
            var oldProperty = new TestPropertyDefinition();
            var newProperty = new TestPropertyDefinition();
            var oldMatchingProperty = new TestPropertyDefinition();
            var oldProperties = new[]
            {
                oldProperty, oldMatchingProperty
            };
            var newMatchingProperty = oldMatchingProperty.JsonClone();
            var newProperties = new[]
            {
                newMatchingProperty, newProperty
            };

            var sut = new PropertyMatchEvaluator();

            var results = sut.MatchItems(oldProperties, newProperties);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldMatchingProperty);
            results.MatchingItems.First().NewItem.Should().Be(newMatchingProperty);
            results.ItemsAdded.Should().HaveCount(1);
            results.ItemsAdded.First().Should().Be(newProperty);
            results.ItemsRemoved.Should().HaveCount(1);
            results.ItemsRemoved.First().Should().Be(oldProperty);
        }

        [Fact]
        public void MatchItemsReturnsSinglePropertyMatchingByName()
        {
            var oldProperty = new TestPropertyDefinition();
            var oldProperties = new[]
            {
                oldProperty
            };
            var newProperty = oldProperty.JsonClone();
            var newProperties = new[]
            {
                newProperty
            };

            var sut = new PropertyMatchEvaluator();

            var results = sut.MatchItems(oldProperties, newProperties);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldProperty);
            results.MatchingItems.First().NewItem.Should().Be(newProperty);
            results.ItemsAdded.Should().BeEmpty();
            results.ItemsRemoved.Should().BeEmpty();
        }

        [Fact]
        public void MatchItemsThrowsExceptionWithNullNewItems()
        {
            var oldItems = Array.Empty<PropertyDefinition>();

            var sut = new PropertyMatchEvaluator();

            Action action = () => sut.MatchItems(oldItems, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MatchItemsThrowsExceptionWithNullOldItems()
        {
            var newItems = Array.Empty<PropertyDefinition>();

            var sut = new PropertyMatchEvaluator();

            Action action = () => sut.MatchItems(null!, newItems);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}