namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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

    public class PropertyComparerTests : Tests<PropertyComparer>
    {
        private readonly ITestOutputHelper _output;

        public PropertyComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareMatchReturnsEmptyWhenPropertiesMatch()
        {
            var oldMember = new TestPropertyDefinition();
            var newMember = oldMember.JsonClone();
            var match = new ItemMatch<IPropertyDefinition>(oldMember, newMember);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchReturnsResultFromPropertyAccessorMatchProcessorWithGetAccessor()
        {
            var oldItem = new TestPropertyDefinition().Set(x => x.SetAccessor = null);
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IPropertyDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] {result};

            Service<IPropertyAccessorMatchProcessor>()
                .CalculateChanges(
                    Arg.Is<IEnumerable<IPropertyAccessorDefinition>>(
                        x => x.Contains(oldItem.GetAccessor)),
                    Arg.Is<IEnumerable<IPropertyAccessorDefinition>>(
                        x => x.Contains(newItem.GetAccessor)),
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public void CompareMatchReturnsResultFromPropertyAccessorMatchProcessorWithGetAndSetAccessors()
        {
            var oldItem = new TestPropertyDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IPropertyDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] {result};

            Service<IPropertyAccessorMatchProcessor>()
                .CalculateChanges(
                    Arg.Is<IEnumerable<IPropertyAccessorDefinition>>(
                        x => x.Contains(oldItem.GetAccessor) && x.Contains(oldItem.SetAccessor)),
                    Arg.Is<IEnumerable<IPropertyAccessorDefinition>>(
                        x => x.Contains(newItem.GetAccessor) && x.Contains(newItem.SetAccessor)),
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }
        
        [Fact]
        public void CompareMatchReturnsResultFromPropertyAccessorMatchProcessorWithMixedAccessors()
        {
            var oldItem = new TestPropertyDefinition().Set(x =>
            {
                x.GetAccessor = new TestPropertyAccessorDefinition();
                x.SetAccessor = null;
            });
            var newItem = oldItem.JsonClone().Set(x =>
            {
                x.GetAccessor = null;
                x.SetAccessor = new TestPropertyAccessorDefinition();
            });
            var match = new ItemMatch<IPropertyDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] {result};

            Service<IPropertyAccessorMatchProcessor>()
                .CalculateChanges(
                    Arg.Is<IEnumerable<IPropertyAccessorDefinition>>(
                        x => x.Contains(oldItem.GetAccessor)),
                    Arg.Is<IEnumerable<IPropertyAccessorDefinition>>(
                        x => x.Contains(newItem.SetAccessor)),
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void CompareMatchReturnsResultFromPropertyAccessorMatchProcessorWithNoAccessors()
        {
            // This doesn't make sense but it could be a scenario that is forced upon the class
            var oldItem = new TestPropertyDefinition().Set(x =>
            {
                x.GetAccessor = null;
                x.SetAccessor = null;
            });
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IPropertyDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] {result};

            Service<IPropertyAccessorMatchProcessor>()
                .CalculateChanges(
                    Arg.Is<IEnumerable<IPropertyAccessorDefinition>>(
                        x => !x.Any()),
                    Arg.Is<IEnumerable<IPropertyAccessorDefinition>>(
                        x => !x.Any()),
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void CompareMatchReturnsResultFromPropertyAccessorMatchProcessorWithSetAccessor()
        {
            var oldItem = new TestPropertyDefinition().Set(x => x.GetAccessor = null);
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IPropertyDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] {result};

            Service<IPropertyAccessorMatchProcessor>()
                .CalculateChanges(
                    Arg.Is<IEnumerable<IPropertyAccessorDefinition>>(
                        x => x.Contains(oldItem.SetAccessor)),
                    Arg.Is<IEnumerable<IPropertyAccessorDefinition>>(
                        x => x.Contains(newItem.SetAccessor)),
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void CompareMatchReturnsResultFromPropertyModifierComparer()
        {
            var item = new TestPropertyDefinition();
            var match = new ItemMatch<IPropertyDefinition>(item, item);
            var options = ComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, item, item, message);
            var results = new[] {result};

            Service<IPropertyModifiersComparer>()
                .CompareMatch(
                    Arg.Is<ItemMatch<IModifiersElement<PropertyModifiers>>>(
                        x => x.OldItem == item && x.NewItem == item),
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }
    }
}