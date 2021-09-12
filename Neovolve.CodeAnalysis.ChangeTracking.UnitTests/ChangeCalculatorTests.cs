namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class ChangeCalculatorTests
    {
        private readonly ILogger _logger;

        public ChangeCalculatorTests(ITestOutputHelper output)
        {
            _logger = output.BuildLogger();
        }

        [Theory]
        [InlineData(SemVerChangeType.Breaking, 1)]
        [InlineData(SemVerChangeType.Feature, 1)]
        [InlineData(SemVerChangeType.None, 0)]
        public void CalculateChangesReturnsChangeTypeFromSingleResult(SemVerChangeType changeType, int expected)
        {
            var options = Model.Create<ComparerOptions>();
            var oldTypes = Array.Empty<TestClassDefinition>();
            var newTypes = Array.Empty<TestClassDefinition>();
            var result = new ComparisonResult(changeType, null, null, Guid.NewGuid().ToString());
            var results = new List<ComparisonResult> {result};

            var processor = Substitute.For<IBaseTypeMatchProcessor>();

            processor.CalculateChanges(oldTypes, newTypes, options).Returns(results);

            var sut = new ChangeCalculator(processor, _logger);

            var actual = sut.CalculateChanges(oldTypes, newTypes, options);

            actual.ChangeType.Should().Be(changeType);
            actual.ComparisonResults.Should().HaveCount(expected);

            if (expected > 0)
            {
                actual.ComparisonResults.Should().Contain(result);
            }
        }

        [Theory]
        [InlineData(SemVerChangeType.None, SemVerChangeType.None, SemVerChangeType.None, 0)]
        [InlineData(SemVerChangeType.None, SemVerChangeType.Feature, SemVerChangeType.Feature, 1)]
        [InlineData(SemVerChangeType.None, SemVerChangeType.Breaking, SemVerChangeType.Breaking, 1)]
        [InlineData(SemVerChangeType.Feature, SemVerChangeType.None, SemVerChangeType.Feature, 1)]
        [InlineData(SemVerChangeType.Feature, SemVerChangeType.Feature, SemVerChangeType.Feature, 2)]
        [InlineData(SemVerChangeType.Feature, SemVerChangeType.Breaking, SemVerChangeType.Breaking, 2)]
        [InlineData(SemVerChangeType.Breaking, SemVerChangeType.None, SemVerChangeType.Breaking, 1)]
        [InlineData(SemVerChangeType.Breaking, SemVerChangeType.Feature, SemVerChangeType.Breaking, 2)]
        [InlineData(SemVerChangeType.Breaking, SemVerChangeType.Breaking, SemVerChangeType.Breaking, 2)]
        public void CalculateChangesReturnsGreatestChangeTypeFromMultipleResult(SemVerChangeType firstChangeType,
            SemVerChangeType secondChangeType, SemVerChangeType expected, int resultCount)
        {
            var options = Model.Create<ComparerOptions>();
            var oldTypes = Array.Empty<TestClassDefinition>();
            var newTypes = Array.Empty<TestClassDefinition>();
            var firstResult = new ComparisonResult(firstChangeType, null, null, Guid.NewGuid().ToString());
            var secondResult = new ComparisonResult(secondChangeType, null, null, Guid.NewGuid().ToString());
            var results = new List<ComparisonResult> {firstResult, secondResult};

            var processor = Substitute.For<IBaseTypeMatchProcessor>();

            processor.CalculateChanges(oldTypes, newTypes, options).Returns(results);

            var sut = new ChangeCalculator(processor, _logger);

            var actual = sut.CalculateChanges(oldTypes, newTypes, options);

            actual.ChangeType.Should().Be(expected);
            actual.ComparisonResults.Should().HaveCount(resultCount);

            if (firstChangeType != SemVerChangeType.None)
            {
                actual.ComparisonResults.Should().Contain(firstResult);
            }

            if (secondChangeType != SemVerChangeType.None)
            {
                actual.ComparisonResults.Should().Contain(secondResult);
            }
        }

        [Fact]
        public void CalculateChangesReturnsNoneWhenNoResultsReturned()
        {
            var options = Model.Create<ComparerOptions>();
            var oldTypes = Array.Empty<TestClassDefinition>();
            var newTypes = Array.Empty<TestClassDefinition>();
            var results = Array.Empty<ComparisonResult>();

            var processor = Substitute.For<IBaseTypeMatchProcessor>();

            processor.CalculateChanges(oldTypes, newTypes, options).Returns(results);

            var sut = new ChangeCalculator(processor, _logger);

            var actual = sut.CalculateChanges(oldTypes, newTypes, options);

            actual.ChangeType.Should().Be(SemVerChangeType.None);
            actual.ComparisonResults.Should().BeEmpty();
        }

        [Fact]
        public void CalculateChangesThrowsExceptionWithNullNewTypes()
        {
            var options = Model.Create<ComparerOptions>();
            var oldTypes = Array.Empty<TestClassDefinition>();

            var processor = Substitute.For<IBaseTypeMatchProcessor>();

            var sut = new ChangeCalculator(processor, _logger);

            Action action = () => sut.CalculateChanges(oldTypes, null!, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CalculateChangesThrowsExceptionWithNullOldTypes()
        {
            var options = Model.Create<ComparerOptions>();
            var newTypes = Array.Empty<TestClassDefinition>();

            var processor = Substitute.For<IBaseTypeMatchProcessor>();

            var sut = new ChangeCalculator(processor, _logger);

            Action action = () => sut.CalculateChanges(null!, newTypes, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage(
            "Usage",
            "CA1806:Do not ignore method results",
            Justification = "The constructor is the target of the test")]
        public void DoesNotThrowExceptionWhenCreatedWithNullLogger()
        {
            var processor = Substitute.For<IBaseTypeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ChangeCalculator(processor, null);

            action.Should().NotThrow();
        }

        [Fact]
        [SuppressMessage(
            "Usage",
            "CA1806:Do not ignore method results",
            Justification = "The constructor is the target of the test")]
        public void ThrowsExceptionWhenCreatedWithNullProcessor()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ChangeCalculator(null!, _logger);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}