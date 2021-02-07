namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ChangeTables
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class PropertyAccessorAccessModifierChangeTableTests
    {
        [Theory]
        [ClassData(typeof(EnumCombinationsDataSet<PropertyAccessorAccessModifiers>))]
        public void CalculateChangeHandlesAllPossibleValues(PropertyAccessorAccessModifiers oldValue, PropertyAccessorAccessModifiers newValue)
        {
            var sut = new PropertyAccessorAccessModifiersChangeTable();

            Action action = () => sut.CalculateChange(oldValue, newValue);

            action.Should().NotThrow();
        }

        [Theory]
        [InlineData(PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.None, SemVerChangeType.None)]
        [InlineData(PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.None)]
        [InlineData(PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.Private, SemVerChangeType.None)]
        [InlineData(PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.Feature)]
        [InlineData(PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.Feature)]
        [InlineData(PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.None, SemVerChangeType.None)]
        [InlineData(PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.None)]
        [InlineData(PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.Private, SemVerChangeType.None)]
        [InlineData(PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.Feature)]
        [InlineData(PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.Feature)]
        [InlineData(PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.None, SemVerChangeType.None)]
        [InlineData(PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.None)]
        [InlineData(PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.Private, SemVerChangeType.None)]
        [InlineData(PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.Feature)]
        [InlineData(PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.Feature)]
        [InlineData(PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.Breaking)]
        [InlineData(PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.Private, SemVerChangeType.Breaking)]
        [InlineData(PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.None)]
        [InlineData(PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.None)]
        [InlineData(PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.Breaking)]
        [InlineData(PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.Private, SemVerChangeType.Breaking)]
        [InlineData(PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.None)]
        [InlineData(PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.None)]
        public void CalculateChangeReturnsExpectedValue(
            PropertyAccessorAccessModifiers oldValue,
            PropertyAccessorAccessModifiers newValue,
            SemVerChangeType expected)
        {
            var sut = new PropertyAccessorAccessModifiersChangeTable();

            var actual = sut.CalculateChange(oldValue, newValue);

            actual.Should().Be(expected);
        }
    }
}