namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class MatchProcessorTests : Tests<MatchProcessor<IClassDefinition>>
    {
        public MatchProcessorTests(ITestOutputHelper output) : base(output.BuildLogger(LogLevel.Debug))
        {
        }

        [Fact]
        public void CalculateChangesReturnsEmptyWhenMatchEvaluatorReturnsEmptyMatchResults()
        {
            var oldItems = Array.Empty<IClassDefinition>();
            var newItems = Array.Empty<IClassDefinition>();
            var options = ComparerOptions.Default;
            var matches = new MatchResults<IClassDefinition>(oldItems, newItems);

            Service<IMatchEvaluator<IClassDefinition>>().FindMatches(oldItems, newItems).Returns(matches);

            var actual = SUT.CalculateChanges(oldItems, newItems, options);

            actual.Should().BeEmpty();
        }

        [Theory]
        [InlineData(SemVerChangeType.Breaking, true)]
        [InlineData(SemVerChangeType.Feature, true)]
        [InlineData(SemVerChangeType.None, false)]
        public void CalculateChangesReturnsResultFromComparer(SemVerChangeType changeType, bool expected)
        {
            var oldItem = new TestClassDefinition();
            var oldItems = new List<IClassDefinition>
            {
                oldItem
            };
            var newItem = new TestClassDefinition();
            var newItems = new List<IClassDefinition>
            {
                newItem
            };
            var match = new ItemMatch<IClassDefinition>(oldItem, newItem);
            var matches = new List<ItemMatch<IClassDefinition>> {match};
            var options = ComparerOptions.Default;
            var matchResults = new MatchResults<IClassDefinition>(matches, Array.Empty<IClassDefinition>(),
                Array.Empty<IClassDefinition>());
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new List<ComparisonResult> {result};

            Service<IMatchEvaluator<IClassDefinition>>().FindMatches(oldItems, newItems).Returns(matchResults);
            Service<IItemComparer<IClassDefinition>>().CompareMatch(match, options).Returns(results);

            var actual = SUT.CalculateChanges(oldItems, newItems, options);

            if (expected)
            {
                actual.Should().BeEquivalentTo(results);
            }
            else
            {
                actual.Should().BeEmpty();
            }
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void CalculateChangesReturnsResultsForAddedElementDefinitions(bool isVisible, bool expected)
        {
            var oldItems = Array.Empty<IClassDefinition>();
            var newItem = new TestClassDefinition().Set(x => x.IsVisible = isVisible);
            var newItems = new List<IClassDefinition>
            {
                newItem
            };
            var options = ComparerOptions.Default;
            var matchResults = new MatchResults<IClassDefinition>(Array.Empty<IClassDefinition>(),
                newItems);

            Service<IMatchEvaluator<IClassDefinition>>().FindMatches(oldItems, newItems).Returns(matchResults);

            var actual = SUT.CalculateChanges(oldItems, newItems, options).ToList();

            if (expected)
            {
                actual.Should().HaveCount(1);
                actual[0].ChangeType.Should().Be(SemVerChangeType.Feature);
            }
            else
            {
                actual.Should().BeEmpty();
            }
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void CalculateChangesReturnsResultsForRemovedElementDefinitions(bool isVisible, bool expected)
        {
            var oldItem = new TestClassDefinition().Set(x => x.IsVisible = isVisible);
            var oldItems = new List<IClassDefinition>
            {
                oldItem
            };
            var newItems = Array.Empty<IClassDefinition>();
            var options = ComparerOptions.Default;
            var matchResults = new MatchResults<IClassDefinition>(oldItems,
                Array.Empty<IClassDefinition>());

            Service<IMatchEvaluator<IClassDefinition>>().FindMatches(oldItems, newItems).Returns(matchResults);

            var actual = SUT.CalculateChanges(oldItems, newItems, options).ToList();

            if (expected)
            {
                actual.Should().HaveCount(1);
                actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            }
            else
            {
                actual.Should().BeEmpty();
            }
        }

        [Fact]
        public void CalculateChangesThrowsExceptionWithNullNewItems()
        {
            var oldItems = Array.Empty<IClassDefinition>();
            var options = ComparerOptions.Default;

            Action action = () => SUT.CalculateChanges(oldItems, null!, options).ForceEnumeration();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CalculateChangesThrowsExceptionWithNullOldItems()
        {
            var newItems = Array.Empty<IClassDefinition>();
            var options = ComparerOptions.Default;

            Action action = () => SUT.CalculateChanges(null!, newItems, options).ForceEnumeration();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CalculateChangesThrowsExceptionWithNullOptions()
        {
            var oldItems = Array.Empty<IClassDefinition>();
            var newItems = Array.Empty<IClassDefinition>();

            Action action = () => SUT.CalculateChanges(oldItems, newItems, null!).ForceEnumeration();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullComparer()
        {
            var evaluator = Substitute.For<IMatchEvaluator<IClassDefinition>>();
            var logger = Substitute.For<ILogger>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Wrapper(evaluator, null!, logger);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullEvaluator()
        {
            var comparer = Substitute.For<IItemComparer<IClassDefinition>>();
            var logger = Substitute.For<ILogger>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Wrapper(null!, comparer, logger);

            action.Should().Throw<ArgumentNullException>();
        }

        protected override MatchProcessor<IClassDefinition> BuildSUT(ConstructorInfo constructor,
            object[] parameterValues)
        {
            return (MatchProcessor<IClassDefinition>) Activator.CreateInstance(typeof(Wrapper), parameterValues)!;
        }

        private class Wrapper : MatchProcessor<IClassDefinition>
        {
            public Wrapper(IMatchEvaluator<IClassDefinition> evaluator, IItemComparer<IClassDefinition> comparer,
                ILogger? logger) : base(evaluator, comparer, logger)
            {
            }

            protected override bool IsVisible(IClassDefinition item)
            {
                return item.IsVisible;
            }
        }
    }
}