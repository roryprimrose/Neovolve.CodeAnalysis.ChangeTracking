namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models;
    using Xunit;

    public class AttributeMatchEvaluatorTests
    {
        [Fact]
        public async Task MatchItemsIdentifiesAttributesNotMatching()
        {
            var oldNode = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.SimpleAttribute.Replace("Simple", "Old"))
                .ConfigureAwait(false);
            var newNode = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.SimpleAttribute.Replace("Simple", "New"))
                .ConfigureAwait(false);
            var matchingNode = await TestNode.FindNode<AttributeSyntax>(AttributeDefinitionCode.SimpleAttribute)
                .ConfigureAwait(false);

            var oldAttribute = new AttributeDefinition(oldNode);
            var newAttribute = new AttributeDefinition(newNode);
            var oldMatchingAttribute = new AttributeDefinition(matchingNode);
            var oldAttributes = new[]
            {
                oldAttribute, oldMatchingAttribute
            };
            var newMatchingAttribute = new AttributeDefinition(matchingNode);
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
        public async Task MatchItemsReturnsSingleAttributeMatchingByName()
        {
            var node = await TestNode.FindNode<AttributeSyntax>(AttributeDefinitionCode.SimpleAttribute)
                .ConfigureAwait(false);

            var oldAttribute = new AttributeDefinition(node);
            var oldAttributes = new[]
            {
                oldAttribute
            };
            var newAttribute = new AttributeDefinition(node);
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
        public async Task MatchItemsReturnsSingleAttributeMatchingByNameIgnoringAttributeSuffix()
        {
            var oldNode = await TestNode.FindNode<AttributeSyntax>(AttributeDefinitionCode.SimpleAttribute)
                .ConfigureAwait(false);
            var newNode = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.SimpleAttribute.Replace("Attribute", string.Empty))
                .ConfigureAwait(false);

            var oldAttribute = new AttributeDefinition(oldNode);
            var oldAttributes = new[]
            {
                oldAttribute
            };
            var newAttribute = new AttributeDefinition(newNode);
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