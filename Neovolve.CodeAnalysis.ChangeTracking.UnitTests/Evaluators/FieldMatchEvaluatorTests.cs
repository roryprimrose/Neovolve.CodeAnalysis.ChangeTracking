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

    public class FieldMatchEvaluatorTests
    {
        [Fact]
        public void FindMatchesIdentifiesFieldsNotMatching()
        {
            var executeStrategy = Model.UsingModule<ConfigurationModule>()
                .Ignoring<TestFieldDefinition>(x => x.DeclaringType).Ignoring<TestFieldDefinition>(x => x.Attributes);
            var oldField = executeStrategy.Create<TestFieldDefinition>();
            var newField = executeStrategy.Create<TestFieldDefinition>();
            var oldMatchingField = executeStrategy.Create<TestFieldDefinition>();
            var oldFields = new[]
            {
                oldField, oldMatchingField
            };
            var newMatchingField =
                executeStrategy.Create<TestFieldDefinition>().Set(x => x.Name = oldMatchingField.Name);
            var newFields = new[]
            {
                newMatchingField, newField
            };

            var sut = new FieldMatchEvaluator();

            var results = sut.FindMatches(oldFields, newFields);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldMatchingField);
            results.MatchingItems.First().NewItem.Should().Be(newMatchingField);
            results.ItemsAdded.Should().HaveCount(1);
            results.ItemsAdded.First().Should().Be(newField);
            results.ItemsRemoved.Should().HaveCount(1);
            results.ItemsRemoved.First().Should().Be(oldField);
        }

        [Theory]
        [InlineData("MyName", "MyName", true)]
        [InlineData("MyName", "myname", false)]
        [InlineData("MyName", "SomeOtherName", false)]
        public void FindMatchesReturnsSingleFieldMatchingByName(string firstName, string secondName, bool expected)
        {
            var executeStrategy = Model.UsingModule<ConfigurationModule>()
                .Ignoring<TestFieldDefinition>(x => x.DeclaringType).Ignoring<TestFieldDefinition>(x => x.Attributes);
            var oldField = executeStrategy.Create<TestFieldDefinition>().Set(x => x.Name = firstName);
            var oldFields = new[]
            {
                oldField
            };
            var newField = executeStrategy.Create<TestFieldDefinition>().Set(x => x.Name = secondName);
            var newFields = new[]
            {
                newField
            };

            var sut = new FieldMatchEvaluator();

            var results = sut.FindMatches(oldFields, newFields);

            if (expected)
            {
                results.MatchingItems.Should().HaveCount(1);
                results.MatchingItems.First().OldItem.Should().Be(oldField);
                results.MatchingItems.First().NewItem.Should().Be(newField);
                results.ItemsAdded.Should().BeEmpty();
                results.ItemsRemoved.Should().BeEmpty();
            }
            else
            {
                results.MatchingItems.Should().BeEmpty();
            }
        }

        [Fact]
        public void FindMatchesThrowsExceptionWithNullNewItems()
        {
            var oldItems = Array.Empty<FieldDefinition>();

            var sut = new FieldMatchEvaluator();

            Action action = () => sut.FindMatches(oldItems, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void FindMatchesThrowsExceptionWithNullOldItems()
        {
            var newItems = Array.Empty<FieldDefinition>();

            var sut = new FieldMatchEvaluator();

            Action action = () => sut.FindMatches(null!, newItems);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}