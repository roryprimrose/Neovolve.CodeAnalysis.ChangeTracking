namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Processors
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;
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
            var oldItems = new List<IFieldDefinition>
            {
                new TestFieldDefinition()
            };
            var newItems = new List<IFieldDefinition>
            {
                new TestFieldDefinition()
            };
            var options = ComparerOptions.Default;
            var matchingItems = new List<ItemMatch<IFieldDefinition>>
            {
                new ItemMatch<IFieldDefinition>(new TestFieldDefinition(), new TestFieldDefinition())
            };
            var itemsRemoved = new List<IFieldDefinition>
            {
                new TestFieldDefinition()
            };
            var itemsAdded = new List<IFieldDefinition>
            {
                new TestFieldDefinition()
            };
            var matchResults = new MatchResults<IFieldDefinition>(matchingItems, itemsRemoved, itemsAdded);

            var comparer = Substitute.For<IFieldComparer>();
            var evaluator = Substitute.For<IMatchEvaluator<IFieldDefinition>>();

            evaluator.MatchItems(oldItems, newItems).Returns(matchResults);

            var sut = new FieldMatchProcessor(evaluator, comparer, _logger);

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
            var expected = new List<ComparisonResult>
            {
                result
            };

            var comparer = Substitute.For<IFieldComparer>();
            var evaluator = Substitute.For<IMatchEvaluator<IFieldDefinition>>();

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
            var evaluator = Substitute.For<IMatchEvaluator<IFieldDefinition>>();

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
            var evaluator = Substitute.For<IMatchEvaluator<IFieldDefinition>>();

            var sut = new Wrapper(comparer, evaluator, _logger);

            Action action = () => sut.RunEvaluateMatch(match, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsVisibleReturnsTrue()
        {
            var item = new TestFieldDefinition();

            var comparer = Substitute.For<IFieldComparer>();
            var evaluator = Substitute.For<IMatchEvaluator<IFieldDefinition>>();

            var sut = new Wrapper(comparer, evaluator, _logger);

            var actual = sut.RunIsVisible(item);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullComparer()
        {
            var evaluator = Substitute.For<IMatchEvaluator<IFieldDefinition>>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new FieldMatchProcessor(evaluator, null!, _logger);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : FieldMatchProcessor
        {
            public Wrapper(
                IFieldComparer comparer,
                IMatchEvaluator<IFieldDefinition> evaluator,
                ILogger? logger) : base(evaluator, comparer, logger)
            {
            }

            public IEnumerable<ComparisonResult> RunEvaluateMatch(
                ItemMatch<IFieldDefinition> match,
                ComparerOptions options)
            {
                return base.EvaluateMatch(match, options);
            }

            public bool RunIsVisible(IFieldDefinition item)
            {
                return base.IsVisible(item);
            }
        }
    }
}