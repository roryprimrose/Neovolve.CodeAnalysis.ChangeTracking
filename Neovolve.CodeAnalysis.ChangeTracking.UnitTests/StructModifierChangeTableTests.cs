namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class StructModifierChangeTableTests
    {
        [Theory]
        [InlineData(StructModifiers.None, StructModifiers.None, SemVerChangeType.None)]
        [InlineData(StructModifiers.None, StructModifiers.ReadOnly, SemVerChangeType.Breaking)]
        [InlineData(StructModifiers.None, StructModifiers.Partial, SemVerChangeType.None)]
        [InlineData(StructModifiers.None, StructModifiers.ReadOnlyPartial, SemVerChangeType.Breaking)]
        [InlineData(StructModifiers.ReadOnly, StructModifiers.None, SemVerChangeType.Feature)]
        [InlineData(StructModifiers.ReadOnly, StructModifiers.ReadOnly, SemVerChangeType.None)]
        [InlineData(StructModifiers.ReadOnly, StructModifiers.Partial, SemVerChangeType.Feature)]
        [InlineData(StructModifiers.ReadOnly, StructModifiers.ReadOnlyPartial, SemVerChangeType.None)]
        [InlineData(StructModifiers.Partial, StructModifiers.None, SemVerChangeType.None)]
        [InlineData(StructModifiers.Partial, StructModifiers.ReadOnly, SemVerChangeType.Breaking)]
        [InlineData(StructModifiers.Partial, StructModifiers.Partial, SemVerChangeType.None)]
        [InlineData(StructModifiers.Partial, StructModifiers.ReadOnlyPartial, SemVerChangeType.Breaking)]
        [InlineData(StructModifiers.ReadOnlyPartial, StructModifiers.None, SemVerChangeType.Feature)]
        [InlineData(StructModifiers.ReadOnlyPartial, StructModifiers.ReadOnly, SemVerChangeType.None)]
        [InlineData(StructModifiers.ReadOnlyPartial, StructModifiers.Partial, SemVerChangeType.Feature)]
        [InlineData(StructModifiers.ReadOnlyPartial, StructModifiers.ReadOnlyPartial, SemVerChangeType.None)]
        public void CalculateChangeReturnsExpectedValueForMemberDefinition(StructModifiers oldValue,
            StructModifiers newValue,
            SemVerChangeType expected)
        {
            var oldItem = Substitute.For<IStructDefinition>();
            var newItem = Substitute.For<IStructDefinition>();

            oldItem.Modifiers.Returns(oldValue);
            newItem.Modifiers.Returns(newValue);

            var match = new ItemMatch<IStructDefinition>(oldItem, newItem);

            var actual = StructModifierChangeTable.CalculateChange(match);

            actual.Should().Be(expected);
        }
    }
}