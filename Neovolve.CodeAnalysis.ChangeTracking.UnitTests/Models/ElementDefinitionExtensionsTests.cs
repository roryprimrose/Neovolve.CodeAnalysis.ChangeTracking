namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class ElementDefinitionExtensionsTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("static", "")]
        [InlineData("public", "public")]
        [InlineData("public static", "public")]
        [InlineData("protected internal", "protected internal")]
        [InlineData("protected internal partial", "protected internal")]
        [InlineData("protected internal static partial", "protected internal")]
        public void GetDeclaredAccessModifiersReturnsAccessModifiersDeclaredOnDefinition(string modifiers,
            string expected)
        {
            var definition = Substitute.For<IElementDefinition>();

            definition.DeclaredModifiers.Returns(modifiers);

            var actual = definition.GetDeclaredAccessModifiers();

            actual.Should().Be(expected);
        }

        [Fact]
        public void GetDeclaredAccessModifiersThrowsExceptionWithNullDefinition()
        {
            Action action = () => ElementDefinitionExtensions.GetDeclaredAccessModifiers(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("public", "")]
        [InlineData("static", "static")]
        [InlineData("public static", "static")]
        [InlineData("protected internal", "")]
        [InlineData("protected internal partial", "partial")]
        [InlineData("protected internal static partial", "static partial")]
        public void GetDeclaredModifiersReturnsModifiersDeclaredOnDefinition(string modifiers, string expected)
        {
            var definition = Substitute.For<IElementDefinition>();

            definition.DeclaredModifiers.Returns(modifiers);

            var actual = definition.GetDeclaredModifiers();

            actual.Should().Be(expected);
        }

        [Fact]
        public void GetDeclaredModifiersThrowsExceptionWithNullDefinition()
        {
            Action action = () => ElementDefinitionExtensions.GetDeclaredModifiers(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}