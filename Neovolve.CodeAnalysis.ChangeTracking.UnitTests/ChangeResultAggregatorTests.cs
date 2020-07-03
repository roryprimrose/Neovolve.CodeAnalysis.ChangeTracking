namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class ChangeResultAggregatorTests
    {
        [Fact]
        public void AddResultsThrowsExceptionWithNullResult()
        {
            var sut = new ChangeResultAggregator();

            Action action = () => sut.AddResults(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddResultThrowsExceptionWithNullResult()
        {
            var sut = new ChangeResultAggregator();

            Action action = () => sut.AddResult(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MergeResultsThrowsExceptionWithNullResult()
        {
            var sut = new ChangeResultAggregator();

            Action action = () => sut.MergeResults(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AssignsDefaultValuesWhenCreated()
        {
            var sut = new ChangeResultAggregator();

            sut.ExitNodeAnalysis.Should().BeFalse();
            sut.Results.Should().BeEmpty();
        }

        [Fact]
        public void MergeResultsCopiesAcrossResultsFromAggregator()
        {
            var childResults = Model.Create<List<ComparisonResult>>();
            var aggregator = new ChangeResultAggregator
            {
                ExitNodeAnalysis = true
            };

            aggregator.AddResults(childResults);

            var sut = new ChangeResultAggregator();

            sut.MergeResults(aggregator);

            var actual = sut.Results;

            actual.Should().BeEquivalentTo(childResults);
            sut.ExitNodeAnalysis.Should().Be(aggregator.ExitNodeAnalysis);
        }

        [Fact]
        public void ResultsReturnsAddedResults()
        {
            var first = ComparisonResult.ItemAdded(new TestPropertyDefinition());
            var second = ComparisonResult.ItemRemoved(new TestFieldDefinition());
            var set = new List<ComparisonResult>
            {
                ComparisonResult.ItemAdded(new TestClassDefinition()),
                ComparisonResult.ItemRemoved(new TestInterfaceDefinition()),
                ComparisonResult.ItemChanged(
                    SemVerChangeType.Feature,
                    new ItemMatch<IPropertyDefinition>(new TestPropertyDefinition(), new TestPropertyDefinition()),
                    Guid.NewGuid().ToString()),
                ComparisonResult.ItemChanged(
                    SemVerChangeType.Breaking,
                    new ItemMatch<IAttributeDefinition>(new TestAttributeDefinition(), new TestAttributeDefinition()),
                    Guid.NewGuid().ToString()),
            };
            var expected = new List<ComparisonResult>
            {
                first
            };

            expected.AddRange(set);
            expected.Add(second);

            var sut = new ChangeResultAggregator();

            sut.AddResult(first);
            sut.AddResults(set);
            sut.AddResult(second);

            var actual = sut.Results;

            actual.Should().BeEquivalentTo(expected);
        }
    }
}