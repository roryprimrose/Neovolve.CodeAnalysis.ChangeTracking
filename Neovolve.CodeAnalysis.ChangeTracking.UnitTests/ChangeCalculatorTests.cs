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
        [InlineData(SemVerChangeType.Breaking)]
        [InlineData(SemVerChangeType.Feature)]
        [InlineData(SemVerChangeType.None)]
        public void CalculateChangesReturnsChangeTypeFromSingleResult(SemVerChangeType changeType)
        {
            var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
            var oldTypes = Array.Empty<TestClassDefinition>();
            var newTypes = Array.Empty<TestClassDefinition>();
            var result = new ComparisonResult(changeType, null, null, Guid.NewGuid().ToString());
            var results = new List<ComparisonResult> {result};

            var processor = Substitute.For<ITypeMatchProcessor>();

            processor.CalculateChanges(oldTypes, newTypes, options).Returns(results);

            var sut = new ChangeCalculator(processor, _logger);

            var actual = sut.CalculateChanges(oldTypes, newTypes, options);

            actual.ChangeType.Should().Be(changeType);
            actual.ComparisonResults.Should().HaveCount(1);
            actual.ComparisonResults.Should().Contain(result);
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
        public void CalculateChangesReturnsGreatestChangeTypeFromMultipleResult(SemVerChangeType firstChangeType,
            SemVerChangeType secondChangeType, SemVerChangeType expected)
        {
            var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
            var oldTypes = Array.Empty<TestClassDefinition>();
            var newTypes = Array.Empty<TestClassDefinition>();
            var firstResult = new ComparisonResult(firstChangeType, null, null, Guid.NewGuid().ToString());
            var secondResult = new ComparisonResult(secondChangeType, null, null, Guid.NewGuid().ToString());
            var results = new List<ComparisonResult> {firstResult, secondResult};

            var processor = Substitute.For<ITypeMatchProcessor>();

            processor.CalculateChanges(oldTypes, newTypes, options).Returns(results);

            var sut = new ChangeCalculator(processor, _logger);

            var actual = sut.CalculateChanges(oldTypes, newTypes, options);

            actual.ChangeType.Should().Be(expected);
            actual.ComparisonResults.Should().HaveCount(2);
            actual.ComparisonResults.Should().Contain(firstResult);
            actual.ComparisonResults.Should().Contain(secondResult);
        }

        [Fact]
        public void CalculateChangesReturnsNoneWhenNoResultsReturned()
        {
            var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
            var oldTypes = Array.Empty<TestClassDefinition>();
            var newTypes = Array.Empty<TestClassDefinition>();
            var results = Array.Empty<ComparisonResult>();

            var processor = Substitute.For<ITypeMatchProcessor>();

            processor.CalculateChanges(oldTypes, newTypes, options).Returns(results);

            var sut = new ChangeCalculator(processor, _logger);

            var actual = sut.CalculateChanges(oldTypes, newTypes, options);

            actual.ChangeType.Should().Be(SemVerChangeType.None);
            actual.ComparisonResults.Should().BeEmpty();
        }

        [Fact]
        public void CalculateChangesThrowsExceptionWithNullNewTypes()
        {
            var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
            var oldTypes = Array.Empty<TestClassDefinition>();

            var processor = Substitute.For<ITypeMatchProcessor>();

            var sut = new ChangeCalculator(processor, _logger);

            Action action = () => sut.CalculateChanges(oldTypes, null!, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CalculateChangesThrowsExceptionWithNullOldTypes()
        {
            var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
            var newTypes = Array.Empty<TestClassDefinition>();

            var processor = Substitute.For<ITypeMatchProcessor>();

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
            var processor = Substitute.For<ITypeMatchProcessor>();

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