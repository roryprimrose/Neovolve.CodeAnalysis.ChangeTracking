namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ChangeTables
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class EnumAccessModifiersChangeTableTests
    {
        [Theory]
        [ClassData(typeof(EnumCombinationsDataSet<EnumAccessModifiers>))]
        public void CalculateChangeHandlesAllPossibleValues(EnumAccessModifiers oldValue, EnumAccessModifiers newValue)
        {
            var sut = new EnumAccessModifiersChangeTable();

            Action action = () => sut.CalculateChange(oldValue, newValue);

            action.Should().NotThrow();
        }

        [Theory]
        // @formatter:off — disable formatter after this line
        [InlineData(EnumAccessModifiers.Internal, EnumAccessModifiers.Internal, SemVerChangeType.None)]
        [InlineData(EnumAccessModifiers.Internal, EnumAccessModifiers.Private, SemVerChangeType.None)]
        [InlineData(EnumAccessModifiers.Internal, EnumAccessModifiers.Protected, SemVerChangeType.Feature)]
        [InlineData(EnumAccessModifiers.Internal, EnumAccessModifiers.Public, SemVerChangeType.Feature)]
        [InlineData(EnumAccessModifiers.Private, EnumAccessModifiers.Internal, SemVerChangeType.None)]
        [InlineData(EnumAccessModifiers.Private, EnumAccessModifiers.Private, SemVerChangeType.None)]
        [InlineData(EnumAccessModifiers.Private, EnumAccessModifiers.Protected, SemVerChangeType.Feature)]
        [InlineData(EnumAccessModifiers.Private, EnumAccessModifiers.Public, SemVerChangeType.Feature)]
        [InlineData(EnumAccessModifiers.Protected, EnumAccessModifiers.Internal, SemVerChangeType.Breaking)]
        [InlineData(EnumAccessModifiers.Protected, EnumAccessModifiers.Private, SemVerChangeType.Breaking)]
        [InlineData(EnumAccessModifiers.Protected, EnumAccessModifiers.Protected, SemVerChangeType.None)]
        [InlineData(EnumAccessModifiers.Protected, EnumAccessModifiers.Public, SemVerChangeType.Feature)]
        [InlineData(EnumAccessModifiers.Public, EnumAccessModifiers.Internal, SemVerChangeType.Breaking)]
        [InlineData(EnumAccessModifiers.Public, EnumAccessModifiers.Private, SemVerChangeType.Breaking)]
        [InlineData(EnumAccessModifiers.Public, EnumAccessModifiers.Protected, SemVerChangeType.Breaking)]
        [InlineData(EnumAccessModifiers.Public, EnumAccessModifiers.Public, SemVerChangeType.None)]
        // @formatter:on — enable formatter after this line
        public void CalculateChangeReturnsExpectedValue(
            EnumAccessModifiers oldValue,
            EnumAccessModifiers newValue,
            SemVerChangeType expected)
        {
            var sut = new EnumAccessModifiersChangeTable();

            var actual = sut.CalculateChange(oldValue, newValue);

            actual.Should().Be(expected);
        }
    }
}