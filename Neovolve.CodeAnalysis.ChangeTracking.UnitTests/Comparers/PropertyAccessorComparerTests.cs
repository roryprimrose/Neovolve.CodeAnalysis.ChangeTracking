namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class PropertyAccessorComparerTests : Tests<PropertyAccessorComparer>
    {
        private readonly ITestOutputHelper _output;

        public PropertyAccessorComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareReturnsBreakingWhenChangedFromSetToInit()
        {
            var oldItem = new TestPropertyAccessorDefinition
            {
                IsVisible = true,
                AccessorType = PropertyAccessorType.Set,
                AccessorPurpose = PropertyAccessorPurpose.Write
            };
            var newItem = new TestPropertyAccessorDefinition
            {
                IsVisible = true,
                AccessorType = PropertyAccessorType.Init,
                AccessorPurpose = PropertyAccessorPurpose.Write
            };
            var match = new ItemMatch<IPropertyAccessorDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual.First().OldItem.Should().Be(oldItem);
            actual.First().NewItem.Should().Be(newItem);
            actual.First().Message.Should().NotBeEmpty();
            actual.First().ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Theory]
        [InlineData(PropertyAccessorPurpose.Read, PropertyAccessorType.Get)]
        [InlineData(PropertyAccessorPurpose.Write, PropertyAccessorType.Set)]
        [InlineData(PropertyAccessorPurpose.Write, PropertyAccessorType.Init)]
        public void CompareReturnsExpectedResultFromPropertyAccessorAccessModifierComparer(
            PropertyAccessorPurpose accessorPurpose, PropertyAccessorType accessorType)
        {
            var oldItem = new TestPropertyAccessorDefinition
            {
                IsVisible = true,
                AccessorType = accessorType,
                AccessorPurpose = accessorPurpose
            };
            var newItem = new TestPropertyAccessorDefinition
            {
                IsVisible = true,
                AccessorType = accessorType,
                AccessorPurpose = accessorPurpose
            };
            var match = new ItemMatch<IPropertyAccessorDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(SemVerChangeType.Breaking, oldItem, newItem, message);
            var results = new List<ComparisonResult> { result };

            Service<IPropertyAccessorAccessModifiersComparer>()
                .CompareMatch(
                    Arg.Is<ItemMatch<IAccessModifiersElement<PropertyAccessorAccessModifiers>>>(x =>
                        x.OldItem == oldItem && x.NewItem == newItem), options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual.First().OldItem.Should().Be(oldItem);
            actual.First().NewItem.Should().Be(newItem);
            actual.First().Message.Should().Be(message);
            actual.First().ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public void CompareReturnsFeatureWhenChangedFromInitToSet()
        {
            var oldItem = new TestPropertyAccessorDefinition
            {
                IsVisible = true,
                AccessorType = PropertyAccessorType.Init,
                AccessorPurpose = PropertyAccessorPurpose.Write
            };
            var newItem = new TestPropertyAccessorDefinition
            {
                IsVisible = true,
                AccessorType = PropertyAccessorType.Set,
                AccessorPurpose = PropertyAccessorPurpose.Write
            };
            var match = new ItemMatch<IPropertyAccessorDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual.First().OldItem.Should().Be(oldItem);
            actual.First().NewItem.Should().Be(newItem);
            actual.First().Message.Should().NotBeEmpty();
            actual.First().ChangeType.Should().Be(SemVerChangeType.Feature);
        }
    }
}