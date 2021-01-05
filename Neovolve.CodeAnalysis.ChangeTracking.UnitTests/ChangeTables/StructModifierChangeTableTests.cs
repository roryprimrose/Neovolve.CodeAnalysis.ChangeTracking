namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ChangeTables
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class StructModifierChangeTableTests
    {
        [Theory]
        [ClassData(typeof(EnumCombinationsDataSet<StructModifiers>))]
        public void CalculateChangeHandlesAllPossibleValues(StructModifiers oldValue, StructModifiers newValue)
        {
            var sut = new StructModifiersChangeTable();

            Action action = () => sut.CalculateChange(oldValue, newValue);

            action.Should().NotThrow();
        }

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
            var sut = new StructModifiersChangeTable();

            var actual = sut.CalculateChange(oldValue, newValue);

            actual.Should().Be(expected);
        }
    }
}