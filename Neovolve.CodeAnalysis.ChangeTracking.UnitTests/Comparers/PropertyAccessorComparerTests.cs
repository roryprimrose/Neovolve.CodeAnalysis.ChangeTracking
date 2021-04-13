namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
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
        public void CompareReturnsExpectedResultFromPropertyAccessorAccessModifierComparer()
        {
            var oldItem = Model.UsingModule<ConfigurationModule>().Create<TestPropertyAccessorDefinition>().Set(x =>
            {
                x.IsVisible = true;
            });
            var newItem = Model.UsingModule<ConfigurationModule>().Create<TestPropertyAccessorDefinition>().Set(x =>
            {
                x.IsVisible = true;
            });
            var match = new ItemMatch<IPropertyAccessorDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(SemVerChangeType.Breaking, oldItem, newItem, message);
            var results = new List<ComparisonResult> {result};

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
    }
}