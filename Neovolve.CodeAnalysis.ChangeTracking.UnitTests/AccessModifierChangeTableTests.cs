namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class AccessModifierChangeTableTests
    {
        [Theory]
        [InlineData(AccessModifier.Internal, AccessModifier.Internal, SemVerChangeType.None)]
        [InlineData(AccessModifier.Internal, AccessModifier.Private, SemVerChangeType.None)]
        [InlineData(AccessModifier.Internal, AccessModifier.Protected, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.Internal, AccessModifier.Public, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.Internal, AccessModifier.ProtectedInternal, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.Internal, AccessModifier.ProtectedPrivate, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.Private, AccessModifier.Internal, SemVerChangeType.None)]
        [InlineData(AccessModifier.Private, AccessModifier.Private, SemVerChangeType.None)]
        [InlineData(AccessModifier.Private, AccessModifier.Protected, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.Private, AccessModifier.Public, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.Private, AccessModifier.ProtectedInternal, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.Private, AccessModifier.ProtectedPrivate, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.Protected, AccessModifier.Internal, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.Protected, AccessModifier.Private, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.Protected, AccessModifier.Protected, SemVerChangeType.None)]
        [InlineData(AccessModifier.Protected, AccessModifier.Public, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.Protected, AccessModifier.ProtectedInternal, SemVerChangeType.None)]
        [InlineData(AccessModifier.Protected, AccessModifier.ProtectedPrivate, SemVerChangeType.None)]
        [InlineData(AccessModifier.Public, AccessModifier.Internal, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.Public, AccessModifier.Private, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.Public, AccessModifier.Protected, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.Public, AccessModifier.Public, SemVerChangeType.None)]
        [InlineData(AccessModifier.Public, AccessModifier.ProtectedInternal, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.Public, AccessModifier.ProtectedPrivate, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.ProtectedInternal, AccessModifier.Internal, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.ProtectedInternal, AccessModifier.Private, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.ProtectedInternal, AccessModifier.Protected, SemVerChangeType.None)]
        [InlineData(AccessModifier.ProtectedInternal, AccessModifier.Public, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.ProtectedInternal, AccessModifier.ProtectedInternal, SemVerChangeType.None)]
        [InlineData(AccessModifier.ProtectedInternal, AccessModifier.ProtectedPrivate, SemVerChangeType.None)]
        [InlineData(AccessModifier.ProtectedPrivate, AccessModifier.Internal, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.ProtectedPrivate, AccessModifier.Private, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.ProtectedPrivate, AccessModifier.Protected, SemVerChangeType.None)]
        [InlineData(AccessModifier.ProtectedPrivate, AccessModifier.Public, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.ProtectedPrivate, AccessModifier.ProtectedInternal, SemVerChangeType.None)]
        [InlineData(AccessModifier.ProtectedPrivate, AccessModifier.ProtectedPrivate, SemVerChangeType.None)]
        public void CalculateChangeReturnsExpectedValueForMemberDefinition(AccessModifier oldValue,
            AccessModifier newValue,
            SemVerChangeType expected)
        {
            var oldItem = Substitute.For<IMemberDefinition>();
            var newItem = Substitute.For<IMemberDefinition>();

            oldItem.AccessModifier.Returns(oldValue);
            newItem.AccessModifier.Returns(newValue);

            var match = new ItemMatch<IMemberDefinition>(oldItem, newItem);

            var actual = AccessModifierChangeTable.CalculateChange(match);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(AccessModifier.Internal, AccessModifier.Internal, SemVerChangeType.None)]
        [InlineData(AccessModifier.Internal, AccessModifier.Private, SemVerChangeType.None)]
        [InlineData(AccessModifier.Internal, AccessModifier.Protected, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.Internal, AccessModifier.Public, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.Internal, AccessModifier.ProtectedInternal, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.Internal, AccessModifier.ProtectedPrivate, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.Private, AccessModifier.Internal, SemVerChangeType.None)]
        [InlineData(AccessModifier.Private, AccessModifier.Private, SemVerChangeType.None)]
        [InlineData(AccessModifier.Private, AccessModifier.Protected, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.Private, AccessModifier.Public, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.Private, AccessModifier.ProtectedInternal, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.Private, AccessModifier.ProtectedPrivate, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.Protected, AccessModifier.Internal, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.Protected, AccessModifier.Private, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.Protected, AccessModifier.Protected, SemVerChangeType.None)]
        [InlineData(AccessModifier.Protected, AccessModifier.Public, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.Protected, AccessModifier.ProtectedInternal, SemVerChangeType.None)]
        [InlineData(AccessModifier.Protected, AccessModifier.ProtectedPrivate, SemVerChangeType.None)]
        [InlineData(AccessModifier.Public, AccessModifier.Internal, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.Public, AccessModifier.Private, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.Public, AccessModifier.Protected, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.Public, AccessModifier.Public, SemVerChangeType.None)]
        [InlineData(AccessModifier.Public, AccessModifier.ProtectedInternal, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.Public, AccessModifier.ProtectedPrivate, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.ProtectedInternal, AccessModifier.Internal, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.ProtectedInternal, AccessModifier.Private, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.ProtectedInternal, AccessModifier.Protected, SemVerChangeType.None)]
        [InlineData(AccessModifier.ProtectedInternal, AccessModifier.Public, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.ProtectedInternal, AccessModifier.ProtectedInternal, SemVerChangeType.None)]
        [InlineData(AccessModifier.ProtectedInternal, AccessModifier.ProtectedPrivate, SemVerChangeType.None)]
        [InlineData(AccessModifier.ProtectedPrivate, AccessModifier.Internal, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.ProtectedPrivate, AccessModifier.Private, SemVerChangeType.Breaking)]
        [InlineData(AccessModifier.ProtectedPrivate, AccessModifier.Protected, SemVerChangeType.None)]
        [InlineData(AccessModifier.ProtectedPrivate, AccessModifier.Public, SemVerChangeType.Feature)]
        [InlineData(AccessModifier.ProtectedPrivate, AccessModifier.ProtectedInternal, SemVerChangeType.None)]
        [InlineData(AccessModifier.ProtectedPrivate, AccessModifier.ProtectedPrivate, SemVerChangeType.None)]
        public void CalculateChangeReturnsExpectedValueForTypeDefinition(AccessModifier oldValue,
            AccessModifier newValue,
            SemVerChangeType expected)
        {
            var oldItem = Substitute.For<ITypeDefinition>();
            var newItem = Substitute.For<ITypeDefinition>();

            oldItem.AccessModifier.Returns(oldValue);
            newItem.AccessModifier.Returns(newValue);

            var match = new ItemMatch<ITypeDefinition>(oldItem, newItem);

            var actual = AccessModifierChangeTable.CalculateChange(match);

            actual.Should().Be(expected);
        }
    }
}