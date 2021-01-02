namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            var oldItems = Model.UsingModule<ConfigurationModule>().Create<List<IAttributeDefinition>>();
            var newItems = Model.UsingModule<ConfigurationModule>().Create<List<IAttributeDefinition>>();
            var options = ComparerOptions.Default;

            options.CompareAttributes = AttributeCompareOption.Skip;

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            var sut = new AttributeMatchProcessor(comparer, evaluator, _logger);

            var actual = sut.CalculateChanges(oldItems, newItems, options);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CalculateChangesReturnsMatchResultsWhenComparingAllAttributes()
        {
            var oldItems = Model.UsingModule<ConfigurationModule>().Create<List<IAttributeDefinition>>();
            var newItems = Model.UsingModule<ConfigurationModule>().Create<List<IAttributeDefinition>>();
            var options = ComparerOptions.Default;
            var matchingItems =
                Model.UsingModule<ConfigurationModule>().Create<List<ItemMatch<IAttributeDefinition>>>();
            var itemsRemoved = Model.UsingModule<ConfigurationModule>().Create<List<IAttributeDefinition>>();
            var itemsAdded = Model.UsingModule<ConfigurationModule>().Create<List<IAttributeDefinition>>();
            var matchResults = new MatchResults<IAttributeDefinition>(matchingItems, itemsRemoved, itemsAdded);

            options.CompareAttributes = AttributeCompareOption.All;

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            evaluator.MatchItems(oldItems, newItems, Arg.Any<Func<IAttributeDefinition, IAttributeDefinition, bool>>())
                .Returns(matchResults);

            var sut = new AttributeMatchProcessor(comparer, evaluator, _logger);

            var actual = sut.CalculateChanges(oldItems, newItems, options);

            actual.Should().NotBeEmpty();
        }

        [Fact]
        public void CalculateChangesReturnsMatchResultsWhenComparingAttributesMatchingOptions()
        {
            var oldItems = Model.UsingModule<ConfigurationModule>().Create<List<TestAttributeDefinition>>()
                .Set(x => x[3].Name = "System.Text.Json.Serialization.JsonPropertyName");
            var newItems = Model.UsingModule<ConfigurationModule>().Create<List<TestAttributeDefinition>>()
                .Set(x => x[5].Name = "JsonIgnoreAttribute");
            var options = ComparerOptions.Default;
            var matchingItems =
                Model.UsingModule<ConfigurationModule>().Create<List<ItemMatch<IAttributeDefinition>>>();
            var itemsRemoved = Model.UsingModule<ConfigurationModule>().Create<List<IAttributeDefinition>>();
            var itemsAdded = Model.UsingModule<ConfigurationModule>().Create<List<IAttributeDefinition>>();
            var matchResults = new MatchResults<IAttributeDefinition>(matchingItems, itemsRemoved, itemsAdded);

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            evaluator.MatchItems(
                    Arg.Is<IEnumerable<IAttributeDefinition>>(x => x.Single() == oldItems[3]),
                    Arg.Is<IEnumerable<IAttributeDefinition>>(x => x.Single() == newItems[5]), Arg.Any<Func<IAttributeDefinition, IAttributeDefinition, bool>>())
                .Returns(matchResults);

            var sut = new AttributeMatchProcessor(comparer, evaluator, _logger);

            var actual = sut.CalculateChanges(oldItems, newItems, options);

            actual.Should().NotBeEmpty();
        }

        [Fact]
        public void CalculateChangesThrowsExceptionWithNullNewItems()
        {
            var oldItems = Model.UsingModule<ConfigurationModule>().Create<List<IAttributeDefinition>>();
            var options = ComparerOptions.Default;

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            var sut = new AttributeMatchProcessor(comparer, evaluator, _logger);

            Action action = () => sut.CalculateChanges(oldItems, null!, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CalculateChangesThrowsExceptionWithNullOldItems()
        {
            var newItems = Model.UsingModule<ConfigurationModule>().Create<List<IAttributeDefinition>>();
            var options = ComparerOptions.Default;

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            var sut = new AttributeMatchProcessor(comparer, evaluator, _logger);

            Action action = () => sut.CalculateChanges(null!, newItems, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CalculateChangesThrowsExceptionWithNullOptions()
        {
            var oldItems = Model.UsingModule<ConfigurationModule>().Create<List<IAttributeDefinition>>();
            var newItems = Model.UsingModule<ConfigurationModule>().Create<List<IAttributeDefinition>>();

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            var sut = new AttributeMatchProcessor(comparer, evaluator, _logger);

            Action action = () => sut.CalculateChanges(oldItems, newItems, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void EvaluateMatchReturnsComparerResult()
        {
            var oldItem = Model.UsingModule<ConfigurationModule>().Create<IAttributeDefinition>();
            var newItem = Model.UsingModule<ConfigurationModule>().Create<IAttributeDefinition>();
            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;
            var result = new ComparisonResult(SemVerChangeType.Breaking, oldItem, newItem, Guid.NewGuid().ToString());
            var expected = new List<ComparisonResult> {result};

            var comparer = Substitute.For<IAttributeComparer>();
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

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            var sut = new Wrapper(comparer, evaluator, _logger);

            Action action = () => sut.RunEvaluateMatch(null!, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void EvaluateMatchThrowsExceptionWithNullOptions()
        {
            var oldItem = Model.UsingModule<ConfigurationModule>().Create<IAttributeDefinition>();
            var newItem = Model.UsingModule<ConfigurationModule>().Create<IAttributeDefinition>();
            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            var sut = new Wrapper(comparer, evaluator, _logger);

            Action action = () => sut.RunEvaluateMatch(match, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("MyName", "MyName", true)]
        [InlineData("MyNameAttribute", "MyNameAttribute", true)]
        [InlineData("MyNameAttribute", "MyName", true)]
        [InlineData("MyName", "MyNameAttribute", true)]
        [InlineData("MyName", "myname", false)]
        [InlineData("MyName", "SomeOtherName", false)]
        public void IsItemMatchReturnsTrueWhenItemNamesMatch(string firstName, string secondName, bool expected)
        {
            var oldItem = Substitute.For<IAttributeDefinition>();
            var newItem = Substitute.For<IAttributeDefinition>();

            oldItem.Name.Returns(firstName);
            newItem.Name.Returns(secondName);

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            var sut = new Wrapper(comparer, evaluator, _logger);

            var actual = sut.RunIsItemMatch(oldItem, newItem);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsItemMatchThrowsExceptionWithNullNewItem()
        {
            var newItem = Model.UsingModule<ConfigurationModule>().Create<IAttributeDefinition>();

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            var sut = new Wrapper(comparer, evaluator, _logger);

            Action action = () => sut.RunIsItemMatch(null!, newItem);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsItemMatchThrowsExceptionWithNullOldItem()
        {
            var oldItem = Model.UsingModule<ConfigurationModule>().Create<IAttributeDefinition>();

            var comparer = Substitute.For<IAttributeComparer>();
            var evaluator = Substitute.For<IMatchEvaluator>();

            var sut = new Wrapper(comparer, evaluator, _logger);

            Action action = () => sut.RunIsItemMatch(oldItem, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsVisibleReturnsTrue()
        {
            var item = Model.UsingModule<ConfigurationModule>().Create<IAttributeDefinition>();

            var comparer = Substitute.For<IAttributeComparer>();
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
            Action action = () => new AttributeMatchProcessor(null!, evaluator, _logger);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : AttributeMatchProcessor
        {
            public Wrapper(IAttributeComparer comparer, IMatchEvaluator evaluator, ILogger? logger) : base(
                comparer,
                evaluator,
                logger)
            {
            }

            public IEnumerable<ComparisonResult> RunEvaluateMatch(
                ItemMatch<IAttributeDefinition> match,
                ComparerOptions options)
            {
                return base.EvaluateMatch(match, options);
            }

            public bool RunIsItemMatch(IAttributeDefinition oldItem, IAttributeDefinition newItem)
            {
                return base.IsItemMatch(oldItem, newItem);
            }

            public bool RunIsVisible(IAttributeDefinition item)
            {
                return base.IsVisible(item);
            }
        }
    }
}