﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class FieldMatchProcessorTests
    {
        private readonly ILogger _logger;
        private readonly ITestOutputHelper _output;

        public FieldMatchProcessorTests(ITestOutputHelper output)
        {
            _output = output;
            _logger = output.BuildLogger();
        }

        [Fact]
        public void CalculateChangesReturnsMatchResultsWhenComparingFields()
        {
            var oldItems = new List<IFieldDefinition> {new TestFieldDefinition()};
            var newItems = new List<IFieldDefinition> {new TestFieldDefinition()};
            var options = ComparerOptions.Default;
            var matchingItems = new List<ItemMatch<IFieldDefinition>>
                {new ItemMatch<IFieldDefinition>(new TestFieldDefinition(), new TestFieldDefinition())};
            var itemsRemoved = new List<IFieldDefinition> {new TestFieldDefinition()};
            var itemsAdded = new List<IFieldDefinition> {new TestFieldDefinition()};
            var matchResults = new MatchResults<IFieldDefinition>(matchingItems, itemsRemoved, itemsAdded);

            var comparer = Substitute.For<IFieldComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            evaluator.MatchItems(oldItems, newItems, Arg.Any<Func<IFieldDefinition, IFieldDefinition, bool>>())
                .Returns(matchResults);

            var sut = new FieldMatchProcessor(comparer, evaluator, _logger);

            var actual = sut.CalculateChanges(oldItems, newItems, options);

            actual.Should().NotBeEmpty();
        }

        [Fact]
        public void EvaluateMatchReturnsComparerResult()
        {
            var oldItem = new TestFieldDefinition();
            var newItem = new TestFieldDefinition();
            var match = new ItemMatch<IFieldDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;
            var result = new ComparisonResult(SemVerChangeType.Breaking, oldItem, newItem, Guid.NewGuid().ToString());
            var expected = new List<ComparisonResult> {result};

            var comparer = Substitute.For<IFieldComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            comparer.CompareItems(match, options).Returns(expected);

            var sut = new Wrapper(comparer, evaluator, _logger);

            var actual = sut.RunEvaluateMatch(match, options);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void EvaluateMatchThrowsExceptionWithNullMatch()
        {
            var options = ComparerOptions.Default;

            var comparer = Substitute.For<IFieldComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            var sut = new Wrapper(comparer, evaluator, _logger);

            Action action = () => sut.RunEvaluateMatch(null!, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void EvaluateMatchThrowsExceptionWithNullOptions()
        {
            var oldItem = new TestFieldDefinition();
            var newItem = new TestFieldDefinition();
            var match = new ItemMatch<IFieldDefinition>(oldItem, newItem);

            var comparer = Substitute.For<IFieldComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            var sut = new Wrapper(comparer, evaluator, _logger);

            Action action = () => sut.RunEvaluateMatch(match, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("MyName", "MyName", true)]
        [InlineData("MyName", "myname", false)]
        [InlineData("MyName", "SomeOtherName", false)]
        public void IsItemMatchReturnsTrueWhenItemNamesMatch(string firstName, string secondName, bool expected)
        {
            var oldItem = Substitute.For<IFieldDefinition>();
            var newItem = Substitute.For<IFieldDefinition>();

            oldItem.Name.Returns(firstName);
            newItem.Name.Returns(secondName);

            var comparer = Substitute.For<IFieldComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            var sut = new Wrapper(comparer, evaluator, _logger);

            var actual = sut.RunIsItemMatch(oldItem, newItem);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsItemMatchThrowsExceptionWithNullNewItem()
        {
            var newItem = new TestFieldDefinition();

            var comparer = Substitute.For<IFieldComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            var sut = new Wrapper(comparer, evaluator, _logger);

            Action action = () => sut.RunIsItemMatch(null!, newItem);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsItemMatchThrowsExceptionWithNullOldItem()
        {
            var oldItem = new TestFieldDefinition();

            var comparer = Substitute.For<IFieldComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            var sut = new Wrapper(comparer, evaluator, _logger);

            Action action = () => sut.RunIsItemMatch(oldItem, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsVisibleReturnsTrue()
        {
            var item = new TestFieldDefinition();

            var comparer = Substitute.For<IFieldComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            var sut = new Wrapper(comparer, evaluator, _logger);

            var actual = sut.RunIsVisible(item);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullComparer()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new FieldMatchProcessor(null!, evaluator, _logger);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : FieldMatchProcessor
        {
            public Wrapper(IFieldComparer comparer, IMatchEvaluator evaluator, ILogger? logger) : base(
                comparer,
                evaluator,
                logger)
            {
            }

            public IEnumerable<ComparisonResult> RunEvaluateMatch(
                ItemMatch<IFieldDefinition> match,
                ComparerOptions options)
            {
                return base.EvaluateMatch(match, options);
            }

            public bool RunIsItemMatch(IFieldDefinition oldItem, IFieldDefinition newItem)
            {
                return base.IsItemMatch(oldItem, newItem);
            }

            public bool RunIsVisible(IFieldDefinition item)
            {
                return base.IsVisible(item);
            }
        }
    }
}