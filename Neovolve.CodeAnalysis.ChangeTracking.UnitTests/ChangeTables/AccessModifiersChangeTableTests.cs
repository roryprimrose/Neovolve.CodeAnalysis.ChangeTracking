namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ChangeTables
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class AccessModifiersChangeTableTests
    {
        [Theory]
        [ClassData(typeof(EnumCombinationsDataSet<AccessModifiers>))]
        public void CalculateChangeHandlesAllPossibleValues(AccessModifiers oldValue, AccessModifiers newValue)
        {
            var sut = new AccessModifiersChangeTable();

            Action action = () => sut.CalculateChange(oldValue, newValue);

            action.Should().NotThrow();
        }

        [Theory]
        // @formatter:off — disable formatter after this line
        [InlineData(AccessModifiers.Internal, AccessModifiers.Internal, SemVerChangeType.None)]
        [InlineData(AccessModifiers.Internal, AccessModifiers.Private, SemVerChangeType.None)]
        [InlineData(AccessModifiers.Internal, AccessModifiers.Protected, SemVerChangeType.Feature)]
        [InlineData(AccessModifiers.Internal, AccessModifiers.Public, SemVerChangeType.Feature)]
        [InlineData(AccessModifiers.Internal, AccessModifiers.ProtectedInternal, SemVerChangeType.Feature)]
        [InlineData(AccessModifiers.Internal, AccessModifiers.ProtectedPrivate, SemVerChangeType.Feature)]
        [InlineData(AccessModifiers.Private, AccessModifiers.Internal, SemVerChangeType.None)]
        [InlineData(AccessModifiers.Private, AccessModifiers.Private, SemVerChangeType.None)]
        [InlineData(AccessModifiers.Private, AccessModifiers.Protected, SemVerChangeType.Feature)]
        [InlineData(AccessModifiers.Private, AccessModifiers.Public, SemVerChangeType.Feature)]
        [InlineData(AccessModifiers.Private, AccessModifiers.ProtectedInternal, SemVerChangeType.Feature)]
        [InlineData(AccessModifiers.Private, AccessModifiers.ProtectedPrivate, SemVerChangeType.Feature)]
        [InlineData(AccessModifiers.Protected, AccessModifiers.Internal, SemVerChangeType.Breaking)]
        [InlineData(AccessModifiers.Protected, AccessModifiers.Private, SemVerChangeType.Breaking)]
        [InlineData(AccessModifiers.Protected, AccessModifiers.Protected, SemVerChangeType.None)]
        [InlineData(AccessModifiers.Protected, AccessModifiers.Public, SemVerChangeType.Feature)]
        [InlineData(AccessModifiers.Protected, AccessModifiers.ProtectedInternal, SemVerChangeType.None)]
        [InlineData(AccessModifiers.Protected, AccessModifiers.ProtectedPrivate, SemVerChangeType.None)]
        [InlineData(AccessModifiers.Public, AccessModifiers.Internal, SemVerChangeType.Breaking)]
        [InlineData(AccessModifiers.Public, AccessModifiers.Private, SemVerChangeType.Breaking)]
        [InlineData(AccessModifiers.Public, AccessModifiers.Protected, SemVerChangeType.Breaking)]
        [InlineData(AccessModifiers.Public, AccessModifiers.Public, SemVerChangeType.None)]
        [InlineData(AccessModifiers.Public, AccessModifiers.ProtectedInternal, SemVerChangeType.Breaking)]
        [InlineData(AccessModifiers.Public, AccessModifiers.ProtectedPrivate, SemVerChangeType.Breaking)]
        [InlineData(AccessModifiers.ProtectedInternal, AccessModifiers.Internal, SemVerChangeType.Breaking)]
        [InlineData(AccessModifiers.ProtectedInternal, AccessModifiers.Private, SemVerChangeType.Breaking)]
        [InlineData(AccessModifiers.ProtectedInternal, AccessModifiers.Protected, SemVerChangeType.None)]
        [InlineData(AccessModifiers.ProtectedInternal, AccessModifiers.Public, SemVerChangeType.Feature)]
        [InlineData(AccessModifiers.ProtectedInternal, AccessModifiers.ProtectedInternal, SemVerChangeType.None)]
        [InlineData(AccessModifiers.ProtectedInternal, AccessModifiers.ProtectedPrivate, SemVerChangeType.None)]
        [InlineData(AccessModifiers.ProtectedPrivate, AccessModifiers.Internal, SemVerChangeType.Breaking)]
        [InlineData(AccessModifiers.ProtectedPrivate, AccessModifiers.Private, SemVerChangeType.Breaking)]
        [InlineData(AccessModifiers.ProtectedPrivate, AccessModifiers.Protected, SemVerChangeType.None)]
        [InlineData(AccessModifiers.ProtectedPrivate, AccessModifiers.Public, SemVerChangeType.Feature)]
        [InlineData(AccessModifiers.ProtectedPrivate, AccessModifiers.ProtectedInternal, SemVerChangeType.None)]
        [InlineData(AccessModifiers.ProtectedPrivate, AccessModifiers.ProtectedPrivate, SemVerChangeType.None)]
        // @formatter:on — enable formatter after this line
        public void CalculateChangeReturnsExpectedValue(
            AccessModifiers oldValue,
            AccessModifiers newValue,
            SemVerChangeType expected)
        {
            var sut = new AccessModifiersChangeTable();

            var actual = sut.CalculateChange(oldValue, newValue);

            actual.Should().Be(expected);
        }
    }
}