namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ChangeTables
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class ParameterModifiersChangeTableTests
    {
        [Theory]
        [ClassData(typeof(EnumCombinationsDataSet<ParameterModifiers>))]
        public void CalculateChangeHandlesAllPossibleValues(ParameterModifiers oldValue, ParameterModifiers newValue)
        {
            var sut = new ParameterModifiersChangeTable();

            Action action = () => sut.CalculateChange(oldValue, newValue);

            action.Should().NotThrow();
        }

        [Theory]
        [InlineData(ParameterModifiers.None, ParameterModifiers.None, SemVerChangeType.None)]
        [InlineData(ParameterModifiers.None, ParameterModifiers.Ref, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifiers.None, ParameterModifiers.Out, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifiers.None, ParameterModifiers.This, SemVerChangeType.Feature)]
        [InlineData(ParameterModifiers.None, ParameterModifiers.Params, SemVerChangeType.Feature)]
        [InlineData(ParameterModifiers.Ref, ParameterModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifiers.Ref, ParameterModifiers.Ref, SemVerChangeType.None)]
        [InlineData(ParameterModifiers.Ref, ParameterModifiers.Out, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifiers.Ref, ParameterModifiers.This, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifiers.Ref, ParameterModifiers.Params, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifiers.Out, ParameterModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifiers.Out, ParameterModifiers.Ref, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifiers.Out, ParameterModifiers.Out, SemVerChangeType.None)]
        [InlineData(ParameterModifiers.Out, ParameterModifiers.This, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifiers.Out, ParameterModifiers.Params, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifiers.This, ParameterModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifiers.This, ParameterModifiers.Ref, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifiers.This, ParameterModifiers.Out, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifiers.This, ParameterModifiers.This, SemVerChangeType.None)]
        [InlineData(ParameterModifiers.This, ParameterModifiers.Params, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifiers.Params, ParameterModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifiers.Params, ParameterModifiers.Ref, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifiers.Params, ParameterModifiers.Out, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifiers.Params, ParameterModifiers.This, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifiers.Params, ParameterModifiers.Params, SemVerChangeType.None)]
        public void CalculateChangeReturnsExpectedValueForMemberDefinition(
            ParameterModifiers oldValue,
            ParameterModifiers newValue,
            SemVerChangeType expected)
        {
            var sut = new ParameterModifiersChangeTable();

            var actual = sut.CalculateChange(oldValue, newValue);

            actual.Should().Be(expected);
        }
    }
}