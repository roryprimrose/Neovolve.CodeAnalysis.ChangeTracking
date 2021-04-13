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

    public class ElementMatchProcessorTests : Tests<ElementMatchProcessor<IClassDefinition>>
    {
        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void CalculateChangesReturnsResultsBasedOnElementVisibility(bool isVisible, bool expected)
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

            Service<IEvaluator<IClassDefinition>>().FindMatches(oldItems, newItems).Returns(matchResults);

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

        protected override ElementMatchProcessor<IClassDefinition> BuildSUT(ConstructorInfo constructor,
            object[] parameterValues)
        {
            return (ElementMatchProcessor<IClassDefinition>) Activator.CreateInstance(typeof(Wrapper),
                parameterValues)!;
        }

        private class Wrapper : ElementMatchProcessor<IClassDefinition>
        {
            public Wrapper(IEvaluator<IClassDefinition> evaluator, IItemComparer<IClassDefinition> comparer,
                ILogger? logger) : base(evaluator, comparer, logger)
            {
            }
        }
    }
}