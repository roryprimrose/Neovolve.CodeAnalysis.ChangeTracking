namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class AttributeMatchEvaluatorTests
    {
        [Fact]
        public void MatchItemsIdentifiesAttributesNotMatching()
        {
            var oldAttribute = Model.UsingModule<ConfigurationModule>().Create<IAttributeDefinition>();
            var newAttribute = Model.UsingModule<ConfigurationModule>().Create<IAttributeDefinition>();
            var oldMatchingAttribute = Model.UsingModule<ConfigurationModule>().Create<IAttributeDefinition>();
            var oldAttributes = new[]
            {
                oldAttribute, oldMatchingAttribute
            };
            var newMatchingAttribute =
                Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>().Set(x => x.Name = oldMatchingAttribute.Name);
            var newAttributes = new[]
            {
                newMatchingAttribute, newAttribute
            };

            var sut = new AttributeMatchEvaluator();

            var results = sut.MatchItems(oldAttributes, newAttributes);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldMatchingAttribute);
            results.MatchingItems.First().NewItem.Should().Be(newMatchingAttribute);
            results.ItemsAdded.Should().HaveCount(1);
            results.ItemsAdded.First().Should().Be(newAttribute);
            results.ItemsRemoved.Should().HaveCount(1);
            results.ItemsRemoved.First().Should().Be(oldAttribute);
        }

        [Fact]
        public void MatchItemsReturnsSingleAttributeMatchingByName()
        {
            var oldAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>();
            var oldAttributes = new[]
            {
                oldAttribute
            };
            var newAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>().Set(x => x.Name = oldAttribute.Name);
            var newAttributes = new[]
            {
                newAttribute
            };

            var sut = new AttributeMatchEvaluator();

            var results = sut.MatchItems(oldAttributes, newAttributes);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldAttribute);
            results.MatchingItems.First().NewItem.Should().Be(newAttribute);
            results.ItemsAdded.Should().BeEmpty();
            results.ItemsRemoved.Should().BeEmpty();
        }

        [Fact]
        public void MatchItemsReturnsSingleAttributeMatchingByNameIgnoringAttributeSuffix()
        {
            var oldAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>().Set(x => x.Name = "SomethingAttribute");
            var oldAttributes = new[]
            {
                oldAttribute
            };
            var newAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>().Set(x => x.Name = "Something");
            var newAttributes = new[]
            {
                newAttribute
            };

            var sut = new AttributeMatchEvaluator();

            var results = sut.MatchItems(oldAttributes, newAttributes);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldAttribute);
            results.MatchingItems.First().NewItem.Should().Be(newAttribute);
            results.ItemsAdded.Should().BeEmpty();
            results.ItemsRemoved.Should().BeEmpty();
        }

        [Fact]
        public void MatchItemsThrowsExceptionWithNullNewItems()
        {
            var oldItems = Array.Empty<AttributeDefinition>();

            var sut = new AttributeMatchEvaluator();

            Action action = () => sut.MatchItems(oldItems, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MatchItemsThrowsExceptionWithNullOldItems()
        {
            var newItems = Array.Empty<AttributeDefinition>();

            var sut = new AttributeMatchEvaluator();

            Action action = () => sut.MatchItems(null!, newItems);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}