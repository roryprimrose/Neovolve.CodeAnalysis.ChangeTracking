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

    public class MethodMatchProcessorTests : Tests<MethodMatchProcessor>
    {
        private readonly ILogger _logger;

        public MethodMatchProcessorTests(ITestOutputHelper output)
        {
            _logger = output.BuildLogger();
        }

        [Theory]
        [InlineData(MethodModifiers.None, SemVerChangeType.Feature)]
        [InlineData(MethodModifiers.Abstract, SemVerChangeType.Breaking)]
        [InlineData(MethodModifiers.New, SemVerChangeType.Feature)]
        [InlineData(MethodModifiers.Override, SemVerChangeType.Feature)]
        [InlineData(MethodModifiers.Sealed, SemVerChangeType.Feature)]
        [InlineData(MethodModifiers.Static, SemVerChangeType.Feature)]
        [InlineData(MethodModifiers.Virtual, SemVerChangeType.Feature)]
        [InlineData(MethodModifiers.Async, SemVerChangeType.Feature)]
        [InlineData(MethodModifiers.AsyncAbstract, SemVerChangeType.Breaking)]
        [InlineData(MethodModifiers.AsyncNew, SemVerChangeType.Feature)]
        [InlineData(MethodModifiers.AsyncOverride, SemVerChangeType.Feature)]
        [InlineData(MethodModifiers.AsyncSealed, SemVerChangeType.Feature)]
        [InlineData(MethodModifiers.AsyncStatic, SemVerChangeType.Feature)]
        [InlineData(MethodModifiers.AsyncVirtual, SemVerChangeType.Feature)]
        [InlineData(MethodModifiers.AbstractOverride, SemVerChangeType.Breaking)]
        [InlineData(MethodModifiers.NewAbstract, SemVerChangeType.Breaking)]
        [InlineData(MethodModifiers.NewAbstractVirtual, SemVerChangeType.Breaking)]
        [InlineData(MethodModifiers.NewStatic, SemVerChangeType.Feature)]
        [InlineData(MethodModifiers.NewVirtual, SemVerChangeType.Feature)]
        [InlineData(MethodModifiers.SealedOverride, SemVerChangeType.Feature)]
        [InlineData(MethodModifiers.AsyncAbstractOverride, SemVerChangeType.Breaking)]
        [InlineData(MethodModifiers.AsyncNewAbstract, SemVerChangeType.Breaking)]
        [InlineData(MethodModifiers.AsyncNewAbstractVirtual, SemVerChangeType.Breaking)]
        [InlineData(MethodModifiers.AsyncNewStatic, SemVerChangeType.Feature)]
        [InlineData(MethodModifiers.AsyncNewVirtual, SemVerChangeType.Feature)]
        [InlineData(MethodModifiers.AsyncSealedOverride, SemVerChangeType.Feature)]
        public void CalculateChangesReturnsResultsForAddedElementDefinitions(MethodModifiers modifiers,
            SemVerChangeType expected)
        {
            var oldItems = Array.Empty<IMethodDefinition>();
            var newItem = new TestMethodDefinition().Set(x =>
            {
                x.IsVisible = true;
                x.Modifiers = modifiers;
            });
            var newItems = new List<IMethodDefinition>
            {
                newItem
            };
            var options = TestComparerOptions.Default;
            var matchResults = new MatchResults<IMethodDefinition>(Array.Empty<IMethodDefinition>(),
                newItems);

            Service<IMethodEvaluator>().FindMatches(oldItems, newItems).Returns(matchResults);

            var actual = SUT.CalculateChanges(oldItems, newItems, options).ToList();

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(expected);
        }

        [Theory]
        [InlineData(false, SemVerChangeType.Breaking)]
        [InlineData(true, SemVerChangeType.Feature)]
        public void CalculateChangesReturnsResultWhenAddingMethodToInterface(bool hasBody, SemVerChangeType expected)
        {
            var declaringType = new TestInterfaceDefinition();
            var oldItems = Array.Empty<IMethodDefinition>();
            var newItem = new TestMethodDefinition().Set(x =>
            {
                x.IsVisible = true;
                x.DeclaringType = declaringType;
                x.HasBody = hasBody;
            });
            var newItems = new List<IMethodDefinition>
            {
                newItem
            };
            var options = TestComparerOptions.Default;
            var matchResults = new MatchResults<IMethodDefinition>(Array.Empty<IMethodDefinition>(),
                newItems);

            Service<IMethodEvaluator>().FindMatches(oldItems, newItems).Returns(matchResults);

            var actual = SUT.CalculateChanges(oldItems, newItems, options).ToList();

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(expected);
        }

        [Fact]
        public void CanCreateClass()
        {
            var comparer = Substitute.For<IMethodComparer>();
            var evaluator = Substitute.For<IMethodEvaluator>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new MethodMatchProcessor(evaluator, comparer, _logger);

            action.Should().NotThrow();
        }
    }
}