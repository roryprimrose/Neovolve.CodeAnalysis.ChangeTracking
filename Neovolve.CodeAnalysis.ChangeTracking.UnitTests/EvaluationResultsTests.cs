namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class EvaluationResultsTests
    {
        [Fact]
        public void CanCreateWithMatches()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition();
            var matchingItem = new ItemMatch<IClassDefinition>(oldItem, newItem);
            var matchingItems = new List<ItemMatch<IClassDefinition>>
            {
                matchingItem
            };
            var removedItem = new TestClassDefinition();
            var removedItems = new List<IClassDefinition>
            {
                removedItem
            };
            var addedItem = new TestClassDefinition();
            var addedItems = new List<IClassDefinition>
            {
                addedItem
            };

            var sut = new EvaluationResults<IClassDefinition>(matchingItems, removedItems, addedItems);

            sut.MatchingItems.Should().BeEquivalentTo(matchingItems);
            sut.ItemsRemoved.Should().BeEquivalentTo(removedItems);
            sut.ItemsAdded.Should().BeEquivalentTo(addedItems);
        }

        [Fact]
        public void CanCreateWithoutMatches()
        {
            var removedItem = new TestClassDefinition();
            var removedItems = new List<IClassDefinition>
            {
                removedItem
            };
            var addedItem = new TestClassDefinition();
            var addedItems = new List<IClassDefinition>
            {
                addedItem
            };

            var sut = new EvaluationResults<IClassDefinition>(removedItems, addedItems);

            sut.MatchingItems.Should().BeEmpty();
            sut.ItemsRemoved.Should().BeEquivalentTo(removedItems);
            sut.ItemsAdded.Should().BeEquivalentTo(addedItems);
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results",
            Justification = "The constructor is the target of the test")]
        public void ThrowsExceptionWhenCreatedWithNullAddedMembersWithMatches()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition();
            var matchingItem = new ItemMatch<IClassDefinition>(oldItem, newItem);
            var matchingItems = new List<ItemMatch<IClassDefinition>>
            {
                matchingItem
            };
            var removedItem = new TestClassDefinition();
            var removedItems = new List<IClassDefinition>
            {
                removedItem
            };

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new EvaluationResults<IClassDefinition>(matchingItems, removedItems, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results",
            Justification = "The constructor is the target of the test")]
        public void ThrowsExceptionWhenCreatedWithNullAddedMembersWithoutMatches()
        {
            var removedItem = new TestClassDefinition();
            var removedItems = new List<IClassDefinition>
            {
                removedItem
            };

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new EvaluationResults<IClassDefinition>(removedItems, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results",
            Justification = "The constructor is the target of the test")]
        public void ThrowsExceptionWhenCreatedWithNullMatches()
        {
            var removedItem = new TestClassDefinition();
            var removedItems = new List<IClassDefinition>
            {
                removedItem
            };
            var addedItem = new TestClassDefinition();
            var addedItems = new List<IClassDefinition>
            {
                addedItem
            };

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new EvaluationResults<IClassDefinition>(null!, removedItems, addedItems);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results",
            Justification = "The constructor is the target of the test")]
        public void ThrowsExceptionWhenCreatedWithNullRemovedMembersWithMatches()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition();
            var matchingItem = new ItemMatch<IClassDefinition>(oldItem, newItem);
            var matchingItems = new List<ItemMatch<IClassDefinition>>
            {
                matchingItem
            };
            var addedItem = new TestClassDefinition();
            var addedItems = new List<IClassDefinition>
            {
                addedItem
            };

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new EvaluationResults<IClassDefinition>(matchingItems, null!, addedItems);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results",
            Justification = "The constructor is the target of the test")]
        public void ThrowsExceptionWhenCreatedWithNullRemovedMembersWithoutMatches()
        {
            var addedItem = new TestClassDefinition();
            var addedItems = new List<IClassDefinition>
            {
                addedItem
            };

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new EvaluationResults<IClassDefinition>(null!, addedItems);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}