namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class IsVisibleExtensionsTests
    {
        [Theory]
        [InlineData(AccessModifiers.Public, true)]
        [InlineData(AccessModifiers.Protected, true)]
        [InlineData(AccessModifiers.ProtectedInternal, true)]
        [InlineData(AccessModifiers.ProtectedPrivate, true)]
        [InlineData(AccessModifiers.Internal, false)]
        [InlineData(AccessModifiers.Private, false)]
        public void IsVisibleForAccessModifiersReturnsExpectedValue(AccessModifiers accessModifiers, bool expected)
        {
            var actual = accessModifiers.IsVisible();

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsVisibleForAccessModifiersThrowsExceptionWithInvalidValue()
        {
            var accessModifier = (AccessModifiers)int.MaxValue;

            Action action = () => { accessModifier.IsVisible(); };

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(EnumAccessModifiers.Public, true)]
        [InlineData(EnumAccessModifiers.Protected, true)]
        [InlineData(EnumAccessModifiers.Internal, false)]
        [InlineData(EnumAccessModifiers.Private, false)]
        public void IsVisibleForEnumAccessModifiersReturnsExpectedValue(EnumAccessModifiers accessModifiers,
            bool expected)
        {
            var actual = accessModifiers.IsVisible();

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsVisibleForEnumAccessModifiersThrowsExceptionWithInvalidValue()
        {
            var accessModifier = (EnumAccessModifiers)int.MaxValue;

            Action action = () => { accessModifier.IsVisible(); };

            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}