namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Evaluators
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class FieldEvaluatorTests
    {
        [Fact]
        public void FindMatchesIdentifiesFieldsNotMatching()
        {
            var oldField = new TestFieldDefinition();
            var newField = new TestFieldDefinition();
            var oldMatchingField = new TestFieldDefinition();
            var oldFields = new[]
            {
                oldField, oldMatchingField
            };
            var newMatchingField =
                new TestFieldDefinition {Name = oldMatchingField.Name};
            var newFields = new[]
            {
                newMatchingField, newField
            };

            var sut = new FieldEvaluator();

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
            var oldField = new TestFieldDefinition {Name = firstName};
            var oldFields = new[]
            {
                oldField
            };
            var newField = new TestFieldDefinition {Name = secondName};
            var newFields = new[]
            {
                newField
            };

            var sut = new FieldEvaluator();

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

            var sut = new FieldEvaluator();

            Action action = () => sut.FindMatches(oldItems, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void FindMatchesThrowsExceptionWithNullOldItems()
        {
            var newItems = Array.Empty<FieldDefinition>();

            var sut = new FieldEvaluator();

            Action action = () => sut.FindMatches(null!, newItems);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}