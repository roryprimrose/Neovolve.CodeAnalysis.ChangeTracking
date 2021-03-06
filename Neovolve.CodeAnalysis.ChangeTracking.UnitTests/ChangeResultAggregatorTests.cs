﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
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
        public void AssignsDefaultValuesWhenCreated()
        {
            var sut = new ChangeResultAggregator();

            sut.ExitNodeAnalysis.Should().BeFalse();
            sut.Results.Should().BeEmpty();
        }

        [Fact]
        public void MergeResultsCopiesAcrossResultsFromAggregator()
        {
            var childResults = new List<ComparisonResult>
            {
                new(SemVerChangeType.Feature, null, new TestClassDefinition(), Guid.NewGuid().ToString()),
                new(SemVerChangeType.Breaking, new TestInterfaceDefinition(), null, Guid.NewGuid().ToString()),
                new(
                    SemVerChangeType.Feature,
                    new TestPropertyDefinition(), new TestPropertyDefinition(),
                    Guid.NewGuid().ToString()),
                new(
                    SemVerChangeType.Breaking,
                    new TestFieldDefinition(), new TestFieldDefinition(),
                    Guid.NewGuid().ToString()),
            };
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
        public void MergeResultsThrowsExceptionWithNullResult()
        {
            var sut = new ChangeResultAggregator();

            Action action = () => sut.MergeResults(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(SemVerChangeType.None, SemVerChangeType.None, SemVerChangeType.None)]
        [InlineData(SemVerChangeType.None, SemVerChangeType.Feature, SemVerChangeType.Feature)]
        [InlineData(SemVerChangeType.None, SemVerChangeType.Breaking, SemVerChangeType.Breaking)]
        [InlineData(SemVerChangeType.Feature, SemVerChangeType.None, SemVerChangeType.Feature)]
        [InlineData(SemVerChangeType.Feature, SemVerChangeType.Feature, SemVerChangeType.Feature)]
        [InlineData(SemVerChangeType.Feature, SemVerChangeType.Breaking, SemVerChangeType.Breaking)]
        [InlineData(SemVerChangeType.Breaking, SemVerChangeType.None, SemVerChangeType.Breaking)]
        [InlineData(SemVerChangeType.Breaking, SemVerChangeType.Feature, SemVerChangeType.Breaking)]
        [InlineData(SemVerChangeType.Breaking, SemVerChangeType.Breaking, SemVerChangeType.Breaking)]
        public void OverallChangeTypeReturnsHighestResult(SemVerChangeType firstType,
            SemVerChangeType secondType, SemVerChangeType expected)
        {
            var firstItem = new TestPropertyDefinition();
            var secondItem = new TestPropertyDefinition();
            var match = new ItemMatch<IPropertyDefinition>(firstItem, secondItem);

            var firstResult = new ComparisonResult(firstType, match.OldItem, match.NewItem, Guid.NewGuid().ToString());
            var secondResult =
                new ComparisonResult(secondType, match.OldItem, match.NewItem, Guid.NewGuid().ToString());

            var sut = new ChangeResultAggregator();

            sut.AddResult(firstResult);
            sut.AddResult(secondResult);

            var actual = sut.OverallChangeType;

            actual.Should().Be(expected);
        }

        [Fact]
        public void OverallChangeTypeReturnsNoneWhenNoResultsEntered()
        {
            var sut = new ChangeResultAggregator();

            var actual = sut.OverallChangeType;

            actual.Should().Be(SemVerChangeType.None);
        }

        [Fact]
        public void ResultsReturnsAddedResults()
        {
            var first = new ComparisonResult(SemVerChangeType.Feature, null, new TestPropertyDefinition(), Guid.NewGuid().ToString());
            var second = new ComparisonResult(SemVerChangeType.Breaking, new TestFieldDefinition(), null, Guid.NewGuid().ToString());
            var set = new List<ComparisonResult>
            {
                new(SemVerChangeType.Feature, null, new TestClassDefinition(), Guid.NewGuid().ToString()),
                new(SemVerChangeType.Breaking, new TestInterfaceDefinition(), null, Guid.NewGuid().ToString()),
                new(
                    SemVerChangeType.Feature,
                    new TestPropertyDefinition(), new TestPropertyDefinition(),
                    Guid.NewGuid().ToString()),
                new(
                    SemVerChangeType.Breaking,
                    new TestAttributeDefinition(), new TestAttributeDefinition(),
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