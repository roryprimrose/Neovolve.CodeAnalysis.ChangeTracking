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

    public class PropertyMatchProcessorTests : Tests<PropertyMatchProcessor>
    {
        private readonly ILogger _logger;

        public PropertyMatchProcessorTests(ITestOutputHelper output)
        {
            _logger = output.BuildLogger();
        }

        [Fact]
        public void CalculateChangesReturnsBreakingWhenAddingMethodToInterface()
        {
            var declaringType = new TestInterfaceDefinition();
            var oldItems = Array.Empty<IPropertyDefinition>();
            var newItem = new TestPropertyDefinition().Set(x =>
            {
                x.IsVisible = true;
                x.DeclaringType = declaringType;
            });
            var newItems = new List<IPropertyDefinition>
            {
                newItem
            };
            var options = TestComparerOptions.Default;
            var matchResults = new MatchResults<IPropertyDefinition>(Array.Empty<IPropertyDefinition>(),
                newItems);

            Service<IPropertyEvaluator>().FindMatches(oldItems, newItems).Returns(matchResults);

            var actual = SUT.CalculateChanges(oldItems, newItems, options).ToList();

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Theory]
        [InlineData(PropertyModifiers.None, SemVerChangeType.Feature)]
        [InlineData(PropertyModifiers.Abstract, SemVerChangeType.Breaking)]
        [InlineData(PropertyModifiers.New, SemVerChangeType.Feature)]
        [InlineData(PropertyModifiers.Override, SemVerChangeType.Feature)]
        [InlineData(PropertyModifiers.Sealed, SemVerChangeType.Feature)]
        [InlineData(PropertyModifiers.Static, SemVerChangeType.Feature)]
        [InlineData(PropertyModifiers.Virtual, SemVerChangeType.Feature)]
        [InlineData(PropertyModifiers.AbstractOverride, SemVerChangeType.Breaking)]
        [InlineData(PropertyModifiers.NewAbstract, SemVerChangeType.Breaking)]
        [InlineData(PropertyModifiers.NewAbstractVirtual, SemVerChangeType.Breaking)]
        [InlineData(PropertyModifiers.NewStatic, SemVerChangeType.Feature)]
        [InlineData(PropertyModifiers.NewVirtual, SemVerChangeType.Feature)]
        [InlineData(PropertyModifiers.SealedOverride, SemVerChangeType.Feature)]
        public void CalculateChangesReturnsResultsForAddedElementDefinitions(PropertyModifiers modifiers,
            SemVerChangeType expected)
        {
            var oldItems = Array.Empty<IPropertyDefinition>();
            var newItem = new TestPropertyDefinition().Set(x =>
            {
                x.IsVisible = true;
                x.Modifiers = modifiers;
            });
            var newItems = new List<IPropertyDefinition>
            {
                newItem
            };
            var options = TestComparerOptions.Default;
            var matchResults = new MatchResults<IPropertyDefinition>(Array.Empty<IPropertyDefinition>(),
                newItems);

            Service<IPropertyEvaluator>().FindMatches(oldItems, newItems).Returns(matchResults);

            var actual = SUT.CalculateChanges(oldItems, newItems, options).ToList();

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(expected);
        }

        [Fact]
        public void CanCreateClass()
        {
            var comparer = Substitute.For<IPropertyComparer>();
            var evaluator = Substitute.For<IPropertyEvaluator>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PropertyMatchProcessor(evaluator, comparer, _logger);

            action.Should().NotThrow();
        }
    }
}