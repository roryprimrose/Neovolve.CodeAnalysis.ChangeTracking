namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class TypeMatchEvaluatorTests
    {
        [Fact]
        public void BuildMatchResultsThrowsExceptionWithNullItemsAdded()
        {
            var match = new List<ItemMatch<IItemDefinition>>();
            var itemsAdded = new List<IItemDefinition>();

            var sut = new Wrapper();

            Action action = () => sut.RunBuildMatchResults(match, null!, itemsAdded);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildMatchResultsThrowsExceptionWithNullItemsRemoved()
        {
            var match = new List<ItemMatch<IItemDefinition>>();
            var itemsRemoved = new List<IItemDefinition>();

            var sut = new Wrapper();

            Action action = () => sut.RunBuildMatchResults(match, itemsRemoved, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildMatchResultsThrowsExceptionWithNullMatch()
        {
            var itemsRemoved = new List<IItemDefinition>();
            var itemsAdded = new List<IItemDefinition>();

            var sut = new Wrapper();

            Action action = () => sut.RunBuildMatchResults(null!, itemsRemoved, itemsAdded);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MatchItemsCalculatesMatchBetweenItemsIdentifiedAsChangedNamespace()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition().Set(x => { x.Name = oldItem.Name; });
            var oldItems = new List<ITypeDefinition>
            {
                oldItem
            };
            var newItems = new List<ITypeDefinition>
            {
                newItem
            };

            var sut = new TypeMatchEvaluator();

            var actual = sut.MatchItems(oldItems, newItems,
                (oldDefinition, newDefinition) => oldDefinition.FullRawName == newDefinition.FullRawName);

            actual.ItemsRemoved.Should().BeEmpty();
            actual.ItemsAdded.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);

            var match = actual.MatchingItems.Single();

            match.OldItem.Should().Be(oldItem);
            match.NewItem.Should().Be(newItem);
        }

        [Fact]
        public void MatchItemsReturnsComparerResultWhenChangedItemsAreDifferentName()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition();
            var oldItems = new List<ITypeDefinition>
            {
                oldItem
            };
            var newItems = new List<ITypeDefinition>
            {
                newItem
            };

            var sut = new TypeMatchEvaluator();

            var actual = sut.MatchItems(oldItems, newItems,
                (oldDefinition, newDefinition) => oldDefinition.FullRawName == newDefinition.FullRawName);

            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems);
            actual.ItemsAdded.Should().BeEquivalentTo(newItems);
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void MatchItemsReturnsComparerResultWhenChangedItemsAreDifferentTypes()
        {
            var oldItem = new TestInterfaceDefinition();
            var newItem = new TestClassDefinition().Set(x => { x.Name = oldItem.Name; });
            var oldItems = new List<ITypeDefinition>
            {
                oldItem
            };
            var newItems = new List<ITypeDefinition>
            {
                newItem
            };

            var sut = new TypeMatchEvaluator();

            var actual = sut.MatchItems(oldItems, newItems,
                (oldDefinition, newDefinition) => oldDefinition.FullRawName == newDefinition.FullRawName);

            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems);
            actual.ItemsAdded.Should().BeEquivalentTo(newItems);
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void MatchItemsReturnsComparerResultWhenChangedItemsAreSameNamespace()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition().Set(x =>
            {
                x.Name = oldItem.Name;
                x.Namespace = oldItem.Namespace;
            });
            var oldItems = new List<ITypeDefinition>
            {
                oldItem
            };
            var newItems = new List<ITypeDefinition>
            {
                newItem
            };

            var sut = new TypeMatchEvaluator();

            var actual = sut.MatchItems(oldItems, newItems,
                (oldDefinition, newDefinition) => oldDefinition.FullRawName == newDefinition.FullRawName);

            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems);
            actual.ItemsAdded.Should().BeEquivalentTo(newItems);
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void MatchItemsReturnsComparerResultWhenMultipleNewItemsHaveSameNameAndType()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition().Set(x => { x.Name = oldItem.Name; });
            var otherNewItem = new TestClassDefinition().Set(x => { x.Name = oldItem.Name; });
            var oldItems = new List<ITypeDefinition>
            {
                oldItem
            };
            var newItems = new List<ITypeDefinition>
            {
                newItem,
                otherNewItem
            };

            var sut = new TypeMatchEvaluator();

            var actual = sut.MatchItems(oldItems, newItems,
                (oldDefinition, newDefinition) => oldDefinition.FullRawName == newDefinition.FullRawName);

            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems);
            actual.ItemsAdded.Should().BeEquivalentTo(newItems);
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void MatchItemsReturnsComparerResultWhenMultipleOldItemsHaveSameNameAndType()
        {
            var oldItem = new TestClassDefinition();
            var otherOldItem = new TestClassDefinition().Set(x => { x.Name = oldItem.Name; });
            var newItem = new TestClassDefinition().Set(x => { x.Name = oldItem.Name; });
            var oldItems = new List<ITypeDefinition>
            {
                oldItem,
                otherOldItem
            };
            var newItems = new List<ITypeDefinition>
            {
                newItem
            };

            var sut = new TypeMatchEvaluator();

            var actual = sut.MatchItems(oldItems, newItems,
                (oldDefinition, newDefinition) => oldDefinition.FullRawName == newDefinition.FullRawName);

            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems);
            actual.ItemsAdded.Should().BeEquivalentTo(newItems);
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void MatchItemsReturnsComparerResultWhenNewItemIsNotTypeDefinition()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestFieldDefinition();
            var oldItems = new List<IItemDefinition>
            {
                oldItem
            };
            var newItems = new List<IItemDefinition>
            {
                newItem
            };

            var sut = new TypeMatchEvaluator();

            var actual = sut.MatchItems(oldItems, newItems,
                (oldDefinition, newDefinition) => oldDefinition.Name == newDefinition.Name);

            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems);
            actual.ItemsAdded.Should().BeEquivalentTo(newItems);
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void MatchItemsReturnsComparerResultWhenNoItemsAddedOrRemoved()
        {
            var oldItems = Array.Empty<ITypeDefinition>();
            var newItems = Array.Empty<ITypeDefinition>();

            var sut = new TypeMatchEvaluator();

            var actual = sut.MatchItems(oldItems, newItems,
                (oldItem, newItem) => oldItem.FullRawName == newItem.FullRawName);

            actual.ItemsRemoved.Should().BeEmpty();
            actual.ItemsAdded.Should().BeEmpty();
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void MatchItemsReturnsComparerResultWhenOldItemIsNotTypeDefinition()
        {
            var oldItem = new TestFieldDefinition();
            var newItem = new TestClassDefinition();
            var oldItems = new List<IItemDefinition>
            {
                oldItem
            };
            var newItems = new List<IItemDefinition>
            {
                newItem
            };

            var sut = new TypeMatchEvaluator();

            var actual = sut.MatchItems(oldItems, newItems,
                (oldDefinition, newDefinition) => oldDefinition.Name == newDefinition.Name);

            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems);
            actual.ItemsAdded.Should().BeEquivalentTo(newItems);
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void MatchItemsSkipsNullNewItem()
        {
            var oldItem = new TestClassDefinition();
            var oldItems = new List<ITypeDefinition>
            {
                oldItem
            };
            var newItems = new List<ITypeDefinition>
            {
                null!
            };

            var sut = new TypeMatchEvaluator();

            var actual = sut.MatchItems(oldItems, newItems,
                (oldDefinition, newDefinition) => oldDefinition?.FullRawName == newDefinition?.FullRawName);

            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems);
            actual.ItemsAdded.Should().BeEquivalentTo(newItems);
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void MatchItemsSkipsNullOldItem()
        {
            var newItem = new TestClassDefinition();
            var oldItems = new List<ITypeDefinition>
            {
                null!
            };
            var newItems = new List<ITypeDefinition>
            {
                newItem
            };

            var sut = new TypeMatchEvaluator();

            var actual = sut.MatchItems(oldItems, newItems,
                (oldDefinition, newDefinition) => oldDefinition?.FullRawName == newDefinition?.FullRawName);

            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems);
            actual.ItemsAdded.Should().BeEquivalentTo(newItems);
            actual.MatchingItems.Should().BeEmpty();
        }

        private class Wrapper : TypeMatchEvaluator
        {
            public IMatchResults<T> RunBuildMatchResults<T>(List<ItemMatch<T>> matches, List<T> itemsRemoved,
                List<T> itemsAdded) where T : IItemDefinition
            {
                return BuildMatchResults(matches, itemsRemoved, itemsAdded);
            }
        }
    }
}