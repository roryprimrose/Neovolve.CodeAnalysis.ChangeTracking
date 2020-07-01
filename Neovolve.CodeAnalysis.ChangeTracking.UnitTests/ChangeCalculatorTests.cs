namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
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

        [Fact]
        public void CalculateChangesReturnsNoChangeWithEmptyTypes()
        {
            var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
            var oldTypes = Array.Empty<TestClassDefinition>();
            var newTypes = Array.Empty<TestClassDefinition>();
            var classMatches = Substitute.For<IMatchResults<IClassDefinition>>();
            var interfaceMatches = Substitute.For<IMatchResults<IInterfaceDefinition>>();

            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<ITypeComparer>();

            evaluator.MatchItems(
                Arg.Any<IEnumerable<IClassDefinition>>(),
                Arg.Any<IEnumerable<IClassDefinition>>(),
                Arg.Any<Func<IClassDefinition, IClassDefinition, bool>>()).Returns(classMatches);
            evaluator.MatchItems(
                Arg.Any<IEnumerable<IInterfaceDefinition>>(),
                Arg.Any<IEnumerable<IInterfaceDefinition>>(),
                Arg.Any<Func<IInterfaceDefinition, IInterfaceDefinition, bool>>()).Returns(interfaceMatches);

            var sut = new ChangeCalculator(evaluator, comparer, _logger);

            var actual = sut.CalculateChanges(oldTypes, newTypes, options);

            actual.ChangeType.Should().Be(SemVerChangeType.None);
            actual.ComparisonResults.Should().BeEmpty();
        }

        [Fact]
        public void CalculateChangesThrowsExceptionWithNullNewTypes()
        {
            var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
            var oldTypes = Array.Empty<TestClassDefinition>();

            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<ITypeComparer>();

            var sut = new ChangeCalculator(evaluator, comparer, _logger);

            Action action = () => sut.CalculateChanges(oldTypes, null!, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CalculateChangesThrowsExceptionWithNullOldTypes()
        {
            var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
            var newTypes = Array.Empty<TestClassDefinition>();

            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<ITypeComparer>();

            var sut = new ChangeCalculator(evaluator, comparer, _logger);

            Action action = () => sut.CalculateChanges(null!, newTypes, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage(
            "Usage",
            "CA1806:Do not ignore method results",
            Justification = "Testing constructor guard clause")]
        public void DoesNotThrowExceptionWhenCreatedWithNullLogger()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<ITypeComparer>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ChangeCalculator(evaluator, comparer, null);

            action.Should().NotThrow();
        }

        [Fact]
        [SuppressMessage(
            "Usage",
            "CA1806:Do not ignore method results",
            Justification = "Testing constructor guard clause")]
        public void ThrowsExceptionWhenCreatedWithNullComparer()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ChangeCalculator(evaluator, null!, _logger);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage(
            "Usage",
            "CA1806:Do not ignore method results",
            Justification = "Testing constructor guard clause")]
        public void ThrowsExceptionWhenCreatedWithNullEvaluator()
        {
            var comparer = Substitute.For<ITypeComparer>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ChangeCalculator(null!, comparer, _logger);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}