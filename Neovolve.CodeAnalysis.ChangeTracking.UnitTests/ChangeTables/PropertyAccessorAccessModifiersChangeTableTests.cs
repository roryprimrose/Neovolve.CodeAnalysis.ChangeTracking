namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ChangeTables
{
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class PropertyAccessorAccessModifiersChangeTableTests
    {
        [Theory]

        // @formatter:off — disable formatter after this line
        [InlineData(PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.None, SemVerChangeType.None)]
        [InlineData(PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.Private, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.None, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.Private, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.None, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.Private, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.None, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.Private, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.None, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.Private, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.None )]
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