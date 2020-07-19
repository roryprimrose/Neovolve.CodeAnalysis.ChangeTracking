namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class IsVisibleExtensionsTests
    {
        [Theory]
        [InlineData(AccessModifier.Public, true)]
        [InlineData(AccessModifier.Protected, true)]
        [InlineData(AccessModifier.ProtectedInternal, true)]
        [InlineData(AccessModifier.ProtectedPrivate, true)]
        [InlineData(AccessModifier.Internal, false)]
        [InlineData(AccessModifier.Private, false)]
        public void IsVisibleReturnsExpectedValue(AccessModifier accessModifier, bool expected)
        {
            var actual = accessModifier.IsVisible();

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsVisibleThrowsExceptionWithInvalidValue()
        {
            var accessModifier = (AccessModifier) int.MaxValue;

            Action action = () => { accessModifier.IsVisible(); };

            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}