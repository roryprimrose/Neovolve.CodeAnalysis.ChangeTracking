namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Evaluators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class TypeEvaluatorTests
    {
        [Fact]
        public void FindMatchesCalculatesMatchBetweenItemsIdentifiedAsChangedNamespace()
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

            var sut = new TypeEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsRemoved.Should().BeEmpty();
            actual.ItemsAdded.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);

            var match = actual.MatchingItems.Single();

            match.OldItem.Should().Be(oldItem);
            match.NewItem.Should().Be(newItem);
        }

        [Fact]
        public void FindMatchesReturnsComparerResultWhenChangedItemsAreDifferentName()
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

            var sut = new TypeEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems);
            actual.ItemsAdded.Should().BeEquivalentTo(newItems);
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void FindMatchesReturnsComparerResultWhenChangedItemsAreDifferentTypes()
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

            var sut = new TypeEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems);
            actual.ItemsAdded.Should().BeEquivalentTo(newItems);
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void FindMatchesReturnsComparerResultWhenChangedItemsAreSameNamespace()
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

            var sut = new TypeEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems);
            actual.ItemsAdded.Should().BeEquivalentTo(newItems);
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void FindMatchesReturnsComparerResultWhenMultipleNewItemsHaveSameNameAndType()
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

            var sut = new TypeEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems);
            actual.ItemsAdded.Should().BeEquivalentTo(newItems);
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void FindMatchesReturnsComparerResultWhenMultipleOldItemsHaveSameNameAndType()
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

            var sut = new TypeEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsRemoved.Should().BeEquivalentTo(oldItems);
            actual.ItemsAdded.Should().BeEquivalentTo(newItems);
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void FindMatchesReturnsComparerResultWhenNoItemsAddedOrRemoved()
        {
            var oldItems = Array.Empty<ITypeDefinition>();
            var newItems = Array.Empty<ITypeDefinition>();

            var sut = new TypeEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsRemoved.Should().BeEmpty();
            actual.ItemsAdded.Should().BeEmpty();
            actual.MatchingItems.Should().BeEmpty();
        }
    }
}