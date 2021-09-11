namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

    public class AttributeMatchProcessorTests
    {
        private readonly ILogger _logger;
        private readonly ITestOutputHelper _output;

        public AttributeMatchProcessorTests(ITestOutputHelper output)
        {
            _output = output;
            _logger = output.BuildLogger();
        }

        [Fact]
        public void CalculateChangesReturnsEmptyWhenAttributeComparisonSkipped()
        {
            var oldItems = new List<IAttributeDefinition> {new TestAttributeDefinition()};
            var newItems = new List<IAttributeDefinition> {new TestAttributeDefinition()};
            var options = ComparerOptions.Default;

            options.CompareAttributes = AttributeCompareOption.Skip;

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IAttributeEvaluator>();

            var sut = new AttributeMatchProcessor(evaluator, comparer, _logger);

            var actual = sut.CalculateChanges(oldItems, newItems, options);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CalculateChangesReturnsMatchResultsWhenComparingAllAttributes()
        {
            var oldItems = new List<IAttributeDefinition> {new TestAttributeDefinition()};
            var newItems = new List<IAttributeDefinition> {new TestAttributeDefinition()};
            var options = ComparerOptions.Default;
            var matchingItems = new List<ItemMatch<IAttributeDefinition>>
            {
                new(new TestAttributeDefinition(), new TestAttributeDefinition())
            };
            var itemsRemoved = new List<IAttributeDefinition> {new TestAttributeDefinition()};
            var itemsAdded = new List<IAttributeDefinition> {new TestAttributeDefinition()};
            var matchResults = new MatchResults<IAttributeDefinition>(matchingItems, itemsRemoved, itemsAdded);

            options.CompareAttributes = AttributeCompareOption.All;

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IAttributeEvaluator>();

            evaluator.FindMatches(oldItems, newItems)
                .Returns(matchResults);

            var sut = new AttributeMatchProcessor(evaluator, comparer, _logger);

            var actual = sut.CalculateChanges(oldItems, newItems, options).ToList();

            foreach (var result in actual)
            {
                _output.WriteLine(result.Message);
            }

            actual.Should().NotBeEmpty();
        }

        [Fact]
        public void CalculateChangesReturnsMatchResultsWhenComparingAttributesMatchingOptions()
        {
            var oldItems = new List<TestAttributeDefinition>
            {
                new(),
                new(),
                new(),
                new()
                {
                    Name = "System.Text.Json.Serialization.JsonPropertyName"
                },
                new(),
                new()
            };
            var newItems = new List<TestAttributeDefinition>
            {
                new(),
                new(),
                new(),
                new(),
                new(),
                new()
                {
                    Name = "JsonIgnoreAttribute"
                }
            };
            var options = ComparerOptions.Default;
            var matchingItems = new List<ItemMatch<IAttributeDefinition>>
            {
                new(new TestAttributeDefinition(), new TestAttributeDefinition())
            };
            var itemsRemoved = new List<IAttributeDefinition> {new TestAttributeDefinition()};
            var itemsAdded = new List<IAttributeDefinition> {new TestAttributeDefinition()};
            var matchResults = new MatchResults<IAttributeDefinition>(matchingItems, itemsRemoved, itemsAdded);

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IAttributeEvaluator>();

            evaluator.FindMatches(
                    Arg.Is<IEnumerable<IAttributeDefinition>>(x => x.Single() == oldItems[3]),
                    Arg.Is<IEnumerable<IAttributeDefinition>>(x => x.Single() == newItems[5]))
                .Returns(matchResults);

            var sut = new AttributeMatchProcessor(evaluator, comparer, _logger);

            var actual = sut.CalculateChanges(oldItems, newItems, options).ToList();

            foreach (var result in actual)
            {
                _output.WriteLine(result.Message);
            }

            evaluator.Received().FindMatches(
                Arg.Is<IEnumerable<IAttributeDefinition>>(x => x.Single() == oldItems[3]),
                Arg.Is<IEnumerable<IAttributeDefinition>>(x => x.Single() == newItems[5]));

            actual.Should().NotBeEmpty();
        }

        [Fact]
        public void CalculateChangesThrowsExceptionWithNullNewItems()
        {
            var oldItems = new List<IAttributeDefinition> {new TestAttributeDefinition()};
            var options = ComparerOptions.Default;

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IAttributeEvaluator>();

            var sut = new AttributeMatchProcessor(evaluator, comparer, _logger);

            Action action = () => sut.CalculateChanges(oldItems, null!, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CalculateChangesThrowsExceptionWithNullOldItems()
        {
            var newItems = new List<IAttributeDefinition> {new TestAttributeDefinition()};
            var options = ComparerOptions.Default;

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IAttributeEvaluator>();

            var sut = new AttributeMatchProcessor(evaluator, comparer, _logger);

            Action action = () => sut.CalculateChanges(null!, newItems, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CalculateChangesThrowsExceptionWithNullOptions()
        {
            var oldItems = new List<IAttributeDefinition> {new TestAttributeDefinition()};
            var newItems = new List<IAttributeDefinition> {new TestAttributeDefinition()};

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IAttributeEvaluator>();

            var sut = new AttributeMatchProcessor(evaluator, comparer, _logger);

            Action action = () => sut.CalculateChanges(oldItems, newItems, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void EvaluateMatchReturnsComparerResult()
        {
            var oldItem = new TestAttributeDefinition();
            var newItem = new TestAttributeDefinition();
            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;
            var result = new ComparisonResult(SemVerChangeType.Breaking, oldItem, newItem, Guid.NewGuid().ToString());
            var expected = new List<ComparisonResult> {result};

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IAttributeEvaluator>();

            comparer.CompareMatch(match, options).Returns(expected);

            var sut = new Wrapper(comparer, evaluator, _logger);

            var actual = sut.RunEvaluateMatch(match, options);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void EvaluateMatchThrowsExceptionWithNullMatch()
        {
            var options = ComparerOptions.Default;

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IAttributeEvaluator>();

            var sut = new Wrapper(comparer, evaluator, _logger);

            Action action = () => sut.RunEvaluateMatch(null!, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void EvaluateMatchThrowsExceptionWithNullOptions()
        {
            var oldItem = new TestAttributeDefinition();
            var newItem = new TestAttributeDefinition();
            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IAttributeEvaluator>();

            var sut = new Wrapper(comparer, evaluator, _logger);

            Action action = () => sut.RunEvaluateMatch(match, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsVisibleReturnsTrue()
        {
            var item = new TestAttributeDefinition();

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IAttributeEvaluator>();

            var sut = new Wrapper(comparer, evaluator, _logger);

            var actual = sut.RunIsVisible(item);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullComparer()
        {
            var evaluator = Substitute.For<IAttributeEvaluator>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new AttributeMatchProcessor(evaluator, null!, _logger);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : AttributeMatchProcessor
        {
            public Wrapper(IAttributeComparer comparer, IAttributeEvaluator evaluator, ILogger? logger) : base(
                evaluator,
                comparer,
                logger)
            {
            }

            public IEnumerable<ComparisonResult> RunEvaluateMatch(
                ItemMatch<IAttributeDefinition> match,
                ComparerOptions options)
            {
                return base.EvaluateMatch(match, options);
            }

            public bool RunIsVisible(IAttributeDefinition item)
            {
                return base.IsVisible(item);
            }
        }
    }
}