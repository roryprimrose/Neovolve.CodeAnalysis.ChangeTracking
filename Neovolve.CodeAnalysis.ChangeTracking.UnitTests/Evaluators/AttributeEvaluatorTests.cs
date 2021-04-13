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

    public class AttributeEvaluatorTests
    {
        [Fact]
        public void FindMatchesIdentifiesAttributesNotMatching()
        {
            var oldAttribute = Model.UsingModule<ConfigurationModule>().Create<IAttributeDefinition>();
            var newAttribute = Model.UsingModule<ConfigurationModule>().Create<IAttributeDefinition>();
            var oldMatchingAttribute = Model.UsingModule<ConfigurationModule>().Create<IAttributeDefinition>();
            var oldAttributes = new[]
            {
                oldAttribute, oldMatchingAttribute
            };
            var newMatchingAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Name = oldMatchingAttribute.Name);
            var newAttributes = new[]
            {
                newMatchingAttribute, newAttribute
            };

            var sut = new AttributeEvaluator();

            var results = sut.FindMatches(oldAttributes, newAttributes);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldMatchingAttribute);
            results.MatchingItems.First().NewItem.Should().Be(newMatchingAttribute);
            results.ItemsAdded.Should().HaveCount(1);
            results.ItemsAdded.First().Should().Be(newAttribute);
            results.ItemsRemoved.Should().HaveCount(1);
            results.ItemsRemoved.First().Should().Be(oldAttribute);
        }

        [Theory]
        [InlineData("MyName", "MyName", true)]
        [InlineData("MyNameAttribute", "MyNameAttribute", true)]
        [InlineData("MyNameAttribute", "MyName", true)]
        [InlineData("MyName", "MyNameAttribute", true)]
        [InlineData("MyName", "myname", false)]
        [InlineData("MyName", "SomeOtherName", false)]
        public void FindMatchesReturnsSingleAttributeMatchingByName(string firstName, string secondName, bool expected)
        {
            var oldAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Name = firstName);
            var oldAttributes = new[]
            {
                oldAttribute
            };
            var newAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Name = secondName);
            var newAttributes = new[]
            {
                newAttribute
            };

            var sut = new AttributeEvaluator();

            var results = sut.FindMatches(oldAttributes, newAttributes);

            if (expected)
            {
                results.MatchingItems.Should().HaveCount(1);
                results.MatchingItems.First().OldItem.Should().Be(oldAttribute);
                results.MatchingItems.First().NewItem.Should().Be(newAttribute);
                results.ItemsAdded.Should().BeEmpty();
                results.ItemsRemoved.Should().BeEmpty();
            }
            else
            {
                results.MatchingItems.Should().BeEmpty();
            }
        }

        [Fact]
        public void FindMatchesReturnsSingleAttributeMatchingByNameIgnoringAttributeSuffix()
        {
            var oldAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Name = "SomethingAttribute");
            var oldAttributes = new[]
            {
                oldAttribute
            };
            var newAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Name = "Something");
            var newAttributes = new[]
            {
                newAttribute
            };

            var sut = new AttributeEvaluator();

            var results = sut.FindMatches(oldAttributes, newAttributes);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldAttribute);
            results.MatchingItems.First().NewItem.Should().Be(newAttribute);
            results.ItemsAdded.Should().BeEmpty();
            results.ItemsRemoved.Should().BeEmpty();
        }

        [Fact]
        public void FindMatchesThrowsExceptionWithNullNewItems()
        {
            var oldItems = Array.Empty<AttributeDefinition>();

            var sut = new AttributeEvaluator();

            Action action = () => sut.FindMatches(oldItems, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void FindMatchesThrowsExceptionWithNullOldItems()
        {
            var newItems = Array.Empty<AttributeDefinition>();

            var sut = new AttributeEvaluator();

            Action action = () => sut.FindMatches(null!, newItems);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}