namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class PropertyAccessorComparerTests
    {
        private readonly ITestOutputHelper _output;

        public PropertyAccessorComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [ClassData(typeof(PropertyAccessorAccessModifierDataSet))]
        public void CompareReturnsExpectedResultOnAccessModifiers(
            string oldModifiers,
            string newModifiers,
            SemVerChangeType expected)
        {
            var oldValue = string.IsNullOrWhiteSpace(oldModifiers)
                ? PropertyAccessorAccessModifier.None
                : Enum.Parse<PropertyAccessorAccessModifier>(oldModifiers.Replace(" ", string.Empty), true);
            var newValue = string.IsNullOrWhiteSpace(newModifiers)
                ? PropertyAccessorAccessModifier.None
                : Enum.Parse<PropertyAccessorAccessModifier>(newModifiers.Replace(" ", string.Empty), true);
            var oldItem = Model.UsingModule<ConfigurationModule>().Create<TestPropertyAccessorDefinition>().Set(x =>
            {
                x.AccessModifier = oldValue;
                x.IsVisible = true;
            });
            var newItem = Model.UsingModule<ConfigurationModule>().Create<TestPropertyAccessorDefinition>().Set(x =>
            {
                x.AccessModifier = newValue;
                x.IsVisible = true;
            });
            var match = new ItemMatch<IPropertyAccessorDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            var sut = new PropertyAccessorComparer(attributeProcessor);

            var actual = sut.CompareItems(match, options).ToList();

            if (expected == SemVerChangeType.None)
            {
                actual.Should().BeEmpty();
            }
            else
            {
                actual.Should().HaveCount(1);

                _output.WriteLine(actual.First().Message);

                actual.First().OldItem.Should().Be(oldItem);
                actual.First().NewItem.Should().Be(newItem);
                actual.First().ChangeType.Should().Be(expected);
            }
        }
    }
}