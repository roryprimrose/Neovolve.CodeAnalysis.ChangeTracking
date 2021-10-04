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

    public class ParameterMatchProcessorTests : Tests<ParameterMatchProcessor>
    {
        public ParameterMatchProcessorTests(ITestOutputHelper output) : base(output.BuildLogger())
        {
        }

        [Fact]
        public void CalculateChangesReturnsBreakingWhenAddingParameter()
        {
            var declaringMember = new TestConstructorDefinition();
            var oldItems = Array.Empty<IParameterDefinition>();
            var newItem = new TestParameterDefinition().Set(x =>
            {
                x.IsVisible = true;
                x.DeclaringMember = declaringMember;
            });
            var newItems = new List<IParameterDefinition>
            {
                newItem
            };
            var options = TestComparerOptions.Default;
            var matchResults = new MatchResults<IParameterDefinition>(Array.Empty<IParameterDefinition>(),
                newItems);

            Service<IParameterEvaluator>().FindMatches(oldItems, newItems).Returns(matchResults);

            var actual = SUT.CalculateChanges(oldItems, newItems, options).ToList();

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public void CanCreateClass()
        {
            var comparer = Substitute.For<IParameterComparer>();
            var evaluator = Substitute.For<IParameterEvaluator>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ParameterMatchProcessor(evaluator, comparer, Service<ILogger>());

            action.Should().NotThrow();
        }
    }
}