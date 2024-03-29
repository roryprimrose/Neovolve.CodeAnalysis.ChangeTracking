﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class ConstructorComparerTests : Tests<ConstructorComparer>
    {
        private readonly ITestOutputHelper _output;

        public ConstructorComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Theory]
        [InlineData(ConstructorModifiers.Static, ConstructorModifiers.None)]
        [InlineData(ConstructorModifiers.None, ConstructorModifiers.Static)]
        public void CompareMatchDoesNotContinueEvaluationWhenModifierChangeIsBreaking(ConstructorModifiers oldModifer,
            ConstructorModifiers newModifer)
        {
            var oldItem = new TestConstructorDefinition().Set(x => x.Modifiers = oldModifer);
            var newItem = oldItem.JsonClone().Set(x => x.Modifiers = newModifer);
            var match = new ItemMatch<IConstructorDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);

            Service<IParameterComparer>().DidNotReceive()
                .CompareMatch(Arg.Any<ItemMatch<IParameterDefinition>>(), Arg.Any<ComparerOptions>());
        }

        [Fact]
        public void CompareMatchReturnsEmptyWhenItemsMatch()
        {
            var oldItem = new TestConstructorDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IConstructorDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchReturnsResultFromParameterProcessor()
        {
            var oldParameter = new TestParameterDefinition();
            var oldParameters = new List<TestParameterDefinition> { oldParameter }.AsReadOnly();
            var oldItem = new TestConstructorDefinition().Set(x => x.Parameters = oldParameters);
            var newParameter = new TestParameterDefinition();
            var newParameters = new List<TestParameterDefinition> { newParameter }.AsReadOnly();
            var newItem = oldItem.JsonClone().Set(x => x.Parameters = newParameters);
            var match = new ItemMatch<IConstructorDefinition>(oldItem, newItem);
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] { result };
            var options = TestComparerOptions.Default;

            Service<IParameterMatchProcessor>()
                .CalculateChanges(oldItem.Parameters, newItem.Parameters, options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullParameterComparer()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () =>
                new ConstructorComparer(accessModifiersComparer, null!,
                    attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}