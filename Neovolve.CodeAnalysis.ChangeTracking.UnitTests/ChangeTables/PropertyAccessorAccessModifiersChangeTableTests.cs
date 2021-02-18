namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ChangeTables
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class PropertyAccessorAccessModifiersChangeTableTests
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
        // @formatter:off — disable formatter after this line
        [InlineData(PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.Private, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.None, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.Private, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.None, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.None, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.Private, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.None, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.Private, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.Breaking )]
        // @formatter:on — enable formatter after this line
        public void CalculateChangeReturnsExpectedValue(
            PropertyAccessorAccessModifiers oldModifiers,
            PropertyAccessorAccessModifiers newModifiers,
            SemVerChangeType expected)
        {
            var sut = new PropertyAccessorAccessModifiersChangeTable();

            var actual = sut.CalculateChange(oldModifiers, newModifiers);

            actual.Should().Be(expected);
        }
    }
}