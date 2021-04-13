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
        public void FindMatchesDoesNotReturnMatchOnMovedTypeWhenGenericTypeCountsAreDifferent()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition().Set(x =>
            {
                x.RawName = oldItem.RawName;

                var updatedGenericTypeParameters = new List<string>(x.GenericTypeParameters)
                {
                    Guid.NewGuid().ToString()
                };

                x.GenericTypeParameters = updatedGenericTypeParameters.AsReadOnly();
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
        public void FindMatchesDoesNotReturnMatchWhenDeclaringTypesAreDifferent()
        {
            var oldParent = new TestClassDefinition();
            var newParent = new TestClassDefinition();
            var oldItem = new TestClassDefinition().Set(x => x.DeclaringType = oldParent);
            var newItem = new TestClassDefinition().Set(x =>
            {
                x.RawName = oldItem.RawName;
                x.Namespace = oldItem.Namespace;
                x.DeclaringType = newParent;
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
        public void FindMatchesDoesNotReturnMatchWhenItemsAreDifferentTypes()
        {
            var oldItem = new TestInterfaceDefinition();
            var newItem = new TestClassDefinition().Set(x => { x.RawName = oldItem.RawName; });
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
        public void FindMatchesDoesNotReturnMatchWhenMultipleNewItemsMatchAsPossibleMovedType()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition().Set(x => { x.RawName = oldItem.RawName; });
            var otherNewItem = new TestClassDefinition().Set(x => { x.RawName = oldItem.RawName; });
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
        public void FindMatchesDoesNotReturnMatchWhenMultipleOldItemsMatchAsPossibleMovedType()
        {
            var oldItem = new TestClassDefinition();
            var otherOldItem = new TestClassDefinition().Set(x => { x.RawName = oldItem.RawName; });
            var newItem = new TestClassDefinition().Set(x => { x.RawName = oldItem.RawName; });
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
        public void FindMatchesDoesNotReturnMatchWhenNamespaceAndRawNameAreDifferent()
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
        public void FindMatchesDoesNotReturnMatchWhenNewItemHasDeclaringTypeAndNewItemDoesNot()
        {
            var parentItem = new TestClassDefinition();
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition().Set(x =>
            {
                x.RawName = oldItem.RawName;
                x.Namespace = oldItem.Namespace;
                x.DeclaringType = parentItem;
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
        public void FindMatchesDoesNotReturnMatchWhenNoItemsAddedOrRemoved()
        {
            var oldItems = Array.Empty<ITypeDefinition>();
            var newItems = Array.Empty<ITypeDefinition>();

            var sut = new TypeEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsRemoved.Should().BeEmpty();
            actual.ItemsAdded.Should().BeEmpty();
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void FindMatchesDoesNotReturnMatchWhenOldItemHasDeclaringTypeAndNewItemDoesNot()
        {
            var parentItem = new TestClassDefinition();
            var oldItem = new TestClassDefinition().Set(x => x.DeclaringType = parentItem);
            var newItem = new TestClassDefinition().Set(x =>
            {
                x.RawName = oldItem.RawName;
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
        public void FindMatchesReturnsMatchOnMovedType()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition().Set(x => { x.RawName = oldItem.RawName; });
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
        public void FindMatchesReturnsMatchOnMovedTypeWithDeclaringTypes()
        {
            var oldParent = new TestClassDefinition();
            var newParent = new TestClassDefinition().Set(x => { x.RawName = oldParent.RawName; });
            var oldItem = new TestClassDefinition().Set(x => x.DeclaringType = oldParent);
            var newItem = new TestClassDefinition().Set(x =>
            {
                x.RawName = oldItem.RawName;
                x.DeclaringType = newParent;
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

            actual.ItemsRemoved.Should().BeEmpty();
            actual.ItemsAdded.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);

            var match = actual.MatchingItems.First();

            match.OldItem.Should().Be(oldItem);
            match.NewItem.Should().Be(newItem);
        }

        [Fact]
        public void FindMatchesReturnsMatchWhenTypeHasSameSignatureWithDeclaringType()
        {
            var oldParent = new TestClassDefinition();
            var newParent = new TestClassDefinition().Set(x =>
            {
                x.RawName = oldParent.RawName;
                x.Namespace = oldParent.Namespace;
            });
            var oldItem = new TestClassDefinition().Set(x => x.DeclaringType = oldParent);
            var newItem = new TestClassDefinition().Set(x =>
            {
                x.RawName = oldItem.RawName;
                x.Namespace = oldItem.Namespace;
                x.DeclaringType = newParent;
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

            actual.ItemsRemoved.Should().BeEmpty();
            actual.ItemsAdded.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);

            var match = actual.MatchingItems.First();

            match.OldItem.Should().Be(oldItem);
            match.NewItem.Should().Be(newItem);
        }

        [Fact]
        public void FindMatchesReturnsMatchWhenTypesHaveSameSignatures()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition().Set(x =>
            {
                x.RawName = oldItem.RawName;
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

            actual.ItemsRemoved.Should().BeEmpty();
            actual.ItemsAdded.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);

            var match = actual.MatchingItems.First();

            match.OldItem.Should().Be(oldItem);
            match.NewItem.Should().Be(newItem);
        }
    }
}