namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ChangeTables
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class FieldModifiersChangeTableTests
    {
        [Theory]
        [ClassData(typeof(EnumCombinationsDataSet<FieldModifiers>))]
        public void CalculateChangeHandlesAllPossibleValues(FieldModifiers oldValue, FieldModifiers newValue)
        {
            var sut = new FieldModifiersChangeTable();

            Action action = () => sut.CalculateChange(oldValue, newValue);

            action.Should().NotThrow();
        }

        [Theory]
        [InlineData(FieldModifiers.None, FieldModifiers.None, SemVerChangeType.None)]
        [InlineData(FieldModifiers.None, FieldModifiers.ReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.None, FieldModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.None, FieldModifiers.StaticReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.ReadOnly, FieldModifiers.None, SemVerChangeType.Feature)]
        [InlineData(FieldModifiers.ReadOnly, FieldModifiers.ReadOnly, SemVerChangeType.None)]
        [InlineData(FieldModifiers.ReadOnly, FieldModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.ReadOnly, FieldModifiers.StaticReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.Static, FieldModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.Static, FieldModifiers.ReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.Static, FieldModifiers.Static, SemVerChangeType.None)]
        [InlineData(FieldModifiers.Static, FieldModifiers.StaticReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.StaticReadOnly, FieldModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.StaticReadOnly, FieldModifiers.ReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.StaticReadOnly, FieldModifiers.Static, SemVerChangeType.Feature)]
        [InlineData(FieldModifiers.StaticReadOnly, FieldModifiers.StaticReadOnly, SemVerChangeType.None)]
        public void CalculateChangeReturnsExpectedValue(FieldModifiers oldValue,
            FieldModifiers newValue,
            SemVerChangeType expected)
        {
            var sut = new FieldModifiersChangeTable();

            var actual = sut.CalculateChange(oldValue, newValue);

            actual.Should().Be(expected);
        }
    }
}