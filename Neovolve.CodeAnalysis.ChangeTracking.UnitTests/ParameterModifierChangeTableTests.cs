﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class ParameterModifierChangeTableTests
    {
        [Theory]
        [InlineData(ParameterModifier.None, ParameterModifier.None, SemVerChangeType.None)]
        [InlineData(ParameterModifier.None, ParameterModifier.Ref, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifier.None, ParameterModifier.Out, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifier.None, ParameterModifier.This, SemVerChangeType.Feature)]
        [InlineData(ParameterModifier.None, ParameterModifier.Params, SemVerChangeType.Feature)]
        [InlineData(ParameterModifier.Ref, ParameterModifier.None, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifier.Ref, ParameterModifier.Ref, SemVerChangeType.None)]
        [InlineData(ParameterModifier.Ref, ParameterModifier.Out, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifier.Ref, ParameterModifier.This, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifier.Ref, ParameterModifier.Params, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifier.Out, ParameterModifier.None, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifier.Out, ParameterModifier.Ref, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifier.Out, ParameterModifier.Out, SemVerChangeType.None)]
        [InlineData(ParameterModifier.Out, ParameterModifier.This, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifier.Out, ParameterModifier.Params, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifier.This, ParameterModifier.None, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifier.This, ParameterModifier.Ref, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifier.This, ParameterModifier.Out, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifier.This, ParameterModifier.This, SemVerChangeType.None)]
        [InlineData(ParameterModifier.This, ParameterModifier.Params, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifier.Params, ParameterModifier.None, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifier.Params, ParameterModifier.Ref, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifier.Params, ParameterModifier.Out, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifier.Params, ParameterModifier.This, SemVerChangeType.Breaking)]
        [InlineData(ParameterModifier.Params, ParameterModifier.Params, SemVerChangeType.None)]
        public void CalculateChangeReturnsExpectedValueForMemberDefinition(
            ParameterModifier oldValue,
            ParameterModifier newValue,
            SemVerChangeType expected)
        {
            var oldItem = Substitute.For<IParameterDefinition>();
            var newItem = Substitute.For<IParameterDefinition>();

            oldItem.Modifier.Returns(oldValue);
            newItem.Modifier.Returns(newValue);

            var match = new ItemMatch<IParameterDefinition>(oldItem, newItem);

            var actual = ParameterModifierChangeTable.CalculateChange(match);

            actual.Should().Be(expected);
        }
    }
}