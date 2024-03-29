﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
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

    public class ParameterComparerTests : Tests<ParameterComparer>
    {
        private readonly ITestOutputHelper _output;

        public ParameterComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenDeclaredIndexChanged()
        {
            var oldItem = new TestParameterDefinition();
            var newItem = oldItem.JsonClone().Set(x => x.DeclaredIndex = oldItem.DeclaredIndex + 1);
            var match = new ItemMatch<IParameterDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenTypeAndNameChanged()
        {
            var oldItem = new TestParameterDefinition();
            var newItem = new TestParameterDefinition();
            var match = new ItemMatch<IParameterDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenTypeChanged()
        {
            var oldItem = new TestParameterDefinition();
            var newItem = oldItem.JsonClone().Set(x => x.Type = "NewType");
            var match = new ItemMatch<IParameterDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public void CompareMatchReturnsEmptyWhenNoChangesFound()
        {
            var oldItem = new TestParameterDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IParameterDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("", "", null)]
        [InlineData("  ", "  ", null)]
        [InlineData("old", "old", null)]
        [InlineData("old", "new", null)]
        [InlineData(null, "new", SemVerChangeType.Feature)]
        [InlineData("", "new", SemVerChangeType.Feature)]
        [InlineData("  ", "new", SemVerChangeType.Feature)]
        [InlineData("old", null, SemVerChangeType.Breaking)]
        [InlineData("old", "", SemVerChangeType.Breaking)]
        [InlineData("old", "  ", SemVerChangeType.Breaking)]
        public void CompareMatchReturnsResultBasedOnDefaultValueChanges(string oldValue, string newValue,
            SemVerChangeType? expected)
        {
            var oldItem = new TestParameterDefinition().Set(x => x.DefaultValue = oldValue);
            var newItem = oldItem.JsonClone().Set(x => x.DefaultValue = newValue);
            var match = new ItemMatch<IParameterDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            if (expected == null)
            {
                actual.Should().BeEmpty();
            }
            else
            {
                actual.Should().HaveCount(1);
                actual[0].ChangeType.Should().Be(expected);
            }
        }

        [Fact]
        public void CompareMatchReturnsResultFromParameterModifierComparer()
        {
            var item = new TestParameterDefinition();
            var match = new ItemMatch<IParameterDefinition>(item, item);
            var options = TestComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, item, item, message);
            var results = new[] { result };

            Service<IParameterModifiersComparer>()
                .CompareMatch(
                    Arg.Is<ItemMatch<IModifiersElement<ParameterModifiers>>>(
                        x => x.OldItem == item && x.NewItem == item),
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullParameterModifierComparer()
        {
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            Action action = () => new ParameterComparer(null!, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}