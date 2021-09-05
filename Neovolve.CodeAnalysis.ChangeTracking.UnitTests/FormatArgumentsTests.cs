namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class FormatArgumentsTests
    {
        [Theory]
        [InlineData("this", "{DefinitionType} {Identifier} this")]
        [InlineData(" this ", "{DefinitionType} {Identifier} this")]
        [InlineData("{DefinitionType} this", "{DefinitionType} this")]
        [InlineData("this {Identifier}", "this {Identifier}")]
        [InlineData("this {Identifier} and {DefinitionType} that", "this {Identifier} and {DefinitionType} that")]
        public void MessageFormatIncludesPrefixWhenNeitherDefinitionTypeNorIdentityIncluded(string format,
            string expected)
        {
            var identifier = Guid.NewGuid().ToString();
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();

            var sut = new FormatArguments(format, oldValue, newValue);

            sut.MessageFormat.Should().Be(expected);
        }

        [Fact]
        public void PropertiesReturnConstructorParameters()
        {
            const string messageFormat = MessagePart.DefinitionType + " " + MessagePart.Identifier + " from "
                                         + MessagePart.OldValue + " to " + MessagePart.NewValue;
            var identifier = Guid.NewGuid().ToString();
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();

            var sut = new FormatArguments(messageFormat, oldValue, newValue);

            sut.MessageFormat.Should().Be(messageFormat);
            sut.OldValue.Should().Be(oldValue);
            sut.NewValue.Should().Be(newValue);
        }

        [Fact]
        public void ThrowsExceptionWithNullIdentifier()
        {
            var messageFormat = Guid.NewGuid().ToString();
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new FormatArguments(messageFormat, oldValue, newValue);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullMessageFormat()
        {
            var identifier = Guid.NewGuid().ToString();
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new FormatArguments(null!, oldValue, newValue);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}