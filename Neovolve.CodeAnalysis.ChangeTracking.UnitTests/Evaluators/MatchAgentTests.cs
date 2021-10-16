namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Evaluators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class MatchAgentTests
    {
        [Fact]
        public void MatchOnDoesNotIdentifyMatchWhenConditionFindsMultipleMatchingNewItems()
        {
            var oldItems = new List<IClassDefinition>
            {
                new TestClassDefinition()
            };
            var newItems = new List<IClassDefinition>
            {
                new TestClassDefinition(),
                new TestClassDefinition()
            };

            var sut = new MatchAgent<IClassDefinition>(oldItems, newItems);

            sut.MatchOn((_, _) => true);

            var actual = sut.Results;

            actual.MatchingItems.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems);
            actual.ItemsAdded.Should().BeEquivalentTo(newItems);
        }

        [Fact]
        public void MatchOnDoesNotIdentifyMatchWhenConditionFindsMultipleMatchingOldItems()
        {
            var oldItems = new List<IClassDefinition>
            {
                new TestClassDefinition(),
                new TestClassDefinition()
            };
            var newItems = new List<IClassDefinition>
            {
                new TestClassDefinition()
            };

            var sut = new MatchAgent<IClassDefinition>(oldItems, newItems);

            sut.MatchOn((_, _) => true);

            var actual = sut.Results;

            actual.MatchingItems.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems);
            actual.ItemsAdded.Should().BeEquivalentTo(newItems);
        }

        [Fact]
        public void MatchOnDoesNotIdentifyMatchWhenConditionReturnsFalse()
        {
            var oldItems = new List<IClassDefinition>
            {
                new TestClassDefinition()
            };
            var newItems = new List<IClassDefinition>
            {
                new TestClassDefinition()
            };

            var sut = new MatchAgent<IClassDefinition>(oldItems, newItems);

            sut.MatchOn((_, _) => false);

            var actual = sut.Results;

            actual.MatchingItems.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems);
            actual.ItemsAdded.Should().BeEquivalentTo(newItems);
        }

        [Fact]
        public void MatchOnIdentifiesMatchesByMultipleConditions()
        {
            var oldItems = new List<IClassDefinition>
            {
                new TestClassDefinition(),
                new TestClassDefinition(),
                new TestClassDefinition(),
                new TestClassDefinition()
            };
            var newItems = new List<IClassDefinition>
            {
                new TestClassDefinition(),
                new TestClassDefinition(),
                new TestClassDefinition()
            };

            var sut = new MatchAgent<IClassDefinition>(oldItems, newItems);

            sut.MatchOn((oldItem, newItem) => oldItem == oldItems[2] && newItem == newItems[1]);
            sut.MatchOn((oldItem, newItem) => oldItem == oldItems[1] && newItem == newItems[0]);

            var actual = sut.Results;

            actual.MatchingItems.Should().HaveCount(2);
            actual.MatchingItems.First().OldItem.Should().Be(oldItems[2]);
            actual.MatchingItems.First().NewItem.Should().Be(newItems[1]);
            actual.MatchingItems.Skip(1).First().OldItem.Should().Be(oldItems[1]);
            actual.MatchingItems.Skip(1).First().NewItem.Should().Be(newItems[0]);
            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems.Where(x => x != oldItems[2] && x != oldItems[1]));
            actual.ItemsAdded.Should().BeEquivalentTo(newItems.Where(x => x != newItems[1] && x != newItems[0]));
        }

        [Fact]
        public void MatchOnIdentifiesMatchesWhenConditionReturnsTrue()
        {
            var oldItems = new List<IClassDefinition>
            {
                new TestClassDefinition()
            };
            var newItems = new List<IClassDefinition>
            {
                new TestClassDefinition()
            };

            var sut = new MatchAgent<IClassDefinition>(oldItems, newItems);

            sut.MatchOn((_, _) => true);

            var actual = sut.Results;

            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().Be(oldItems[0]);
            actual.MatchingItems.First().NewItem.Should().Be(newItems[0]);
            actual.ItemsRemoved.Should().BeEmpty();
            actual.ItemsAdded.Should().BeEmpty();
        }

        [Fact]
        public void MatchOnIdentifiesMultipleMatchBySingleCondition()
        {
            var oldItems = new List<IClassDefinition>
            {
                new TestClassDefinition(),
                new TestClassDefinition(),
                new TestClassDefinition(),
                new TestClassDefinition()
            };
            var newItems = new List<IClassDefinition>
            {
                new TestClassDefinition(),
                new TestClassDefinition(),
                new TestClassDefinition()
            };

            var sut = new MatchAgent<IClassDefinition>(oldItems, newItems);

            sut.MatchOn((oldItem, newItem) => oldItem == oldItems[2] && newItem == newItems[1]
                                              || oldItem == oldItems[1] && newItem == newItems[0]);

            var actual = sut.Results;

            actual.MatchingItems.Should().HaveCount(2);
            actual.MatchingItems.First().OldItem.Should().Be(oldItems[2]);
            actual.MatchingItems.First().NewItem.Should().Be(newItems[1]);
            actual.MatchingItems.Skip(1).First().OldItem.Should().Be(oldItems[1]);
            actual.MatchingItems.Skip(1).First().NewItem.Should().Be(newItems[0]);
            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems.Where(x => x != oldItems[2] && x != oldItems[1]));
            actual.ItemsAdded.Should().BeEquivalentTo(newItems.Where(x => x != newItems[1] && x != newItems[0]));
        }

        [Fact]
        public void MatchOnIdentifiesSingleMatchBySingleCondition()
        {
            var oldItems = new List<IClassDefinition>
            {
                new TestClassDefinition(),
                new TestClassDefinition(),
                new TestClassDefinition(),
                new TestClassDefinition()
            };
            var newItems = new List<IClassDefinition>
            {
                new TestClassDefinition(),
                new TestClassDefinition(),
                new TestClassDefinition()
            };

            var sut = new MatchAgent<IClassDefinition>(oldItems, newItems);

            sut.MatchOn((oldItem, newItem) => oldItem == oldItems[2] && newItem == newItems[1]);

            var actual = sut.Results;

            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().Be(oldItems[2]);
            actual.MatchingItems.First().NewItem.Should().Be(newItems[1]);
            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems.Where(x => x != oldItems[2]));
            actual.ItemsAdded.Should().BeEquivalentTo(newItems.Where(x => x != newItems[1]));
        }

        [Fact]
        public void MatchOnSkipsProcessingWhenNoMoreNewItemsFound()
        {
            var oldItems = new List<IClassDefinition>
            {
                new TestClassDefinition(),
                new TestClassDefinition()
            };
            var newItems = new List<IClassDefinition>
            {
                new TestClassDefinition()
            };

            var sut = new MatchAgent<IClassDefinition>(oldItems, newItems);

            sut.MatchOn((oldItem, newItem) => oldItem == oldItems[1] && newItem == newItems[0]);
            sut.MatchOn((_, _) => true);

            var actual = sut.Results;

            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().Be(oldItems[1]);
            actual.MatchingItems.First().NewItem.Should().Be(newItems[0]);
            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems.Where(x => x == oldItems[0]));
            actual.ItemsAdded.Should().BeEmpty();
        }

        [Fact]
        public void MatchOnSkipsProcessingWhenNoMoreOldItemsFound()
        {
            var oldItems = new List<IClassDefinition>
            {
                new TestClassDefinition()
            };
            var newItems = new List<IClassDefinition>
            {
                new TestClassDefinition(),
                new TestClassDefinition()
            };

            var sut = new MatchAgent<IClassDefinition>(oldItems, newItems);

            sut.MatchOn((oldItem, newItem) => oldItem == oldItems[0] && newItem == newItems[1]);
            sut.MatchOn((_, _) => true);

            var actual = sut.Results;

            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().Be(oldItems[0]);
            actual.MatchingItems.First().NewItem.Should().Be(newItems[1]);
            actual.ItemsRemoved.Should().BeEmpty();
            actual.ItemsAdded.Should().BeEquivalentTo(newItems.Where(x => x == newItems[0]));
        }

        [Fact]
        public void MatchOnThrowsExceptionWithNullCondition()
        {
            var oldItems = new List<IClassDefinition>
            {
                new TestClassDefinition()
            };
            var newItems = new List<IClassDefinition>
            {
                new TestClassDefinition()
            };

            var sut = new MatchAgent<IClassDefinition>(oldItems, newItems);

            Action action = () => sut.MatchOn(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ResultsReturnsNewInstance()
        {
            var oldItems = new List<IClassDefinition>
            {
                new TestClassDefinition()
            };
            var newItems = new List<IClassDefinition>
            {
                new TestClassDefinition()
            };

            var sut = new MatchAgent<IClassDefinition>(oldItems, newItems);

            var first = sut.Results;
            var second = sut.Results;

            first.Should().BeEquivalentTo(second);
            second.Should().NotBeSameAs(first);
        }

        [Fact]
        public void ResultsReturnsOriginalItemsWhenNoMatchingAttempted()
        {
            var oldItems = new List<IClassDefinition>
            {
                new TestClassDefinition()
            };
            var newItems = new List<IClassDefinition>
            {
                new TestClassDefinition()
            };

            var sut = new MatchAgent<IClassDefinition>(oldItems, newItems);

            var actual = sut.Results;

            actual.MatchingItems.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems);
            actual.ItemsAdded.Should().BeEquivalentTo(newItems);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullNewItems()
        {
            var oldItems = Array.Empty<IClassDefinition>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new MatchAgent<IClassDefinition>(oldItems, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullOldItems()
        {
            var newItems = Array.Empty<IClassDefinition>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new MatchAgent<IClassDefinition>(null!, newItems);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}