namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class FormatArgumentsTests
    {
        [Theory]
        [InlineData("this", "{Identifier} this")]
        [InlineData(" this ", "{Identifier} this")]
        [InlineData("this {Identifier}", "this {Identifier}")]
        [InlineData("this {Identifier} and that", "this {Identifier} and that")]
        public void MessageFormatIncludesPrefixWhenNeitherDefinitionTypeNorIdentityIncluded(string format,
            string expected)
        {
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();

            var sut = new FormatArguments(format, oldValue, newValue);

            sut.MessageFormat.Should().Be(expected);
        }

        [Fact]
        public void PropertiesReturnConstructorParameters()
        {
            const string messageFormat = MessagePart.Identifier + " from "
                                         + MessagePart.OldValue + " to " + MessagePart.NewValue;
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();

            var sut = new FormatArguments(messageFormat, oldValue, newValue);

            sut.MessageFormat.Should().Be(messageFormat);
            sut.OldValue.Should().Be(oldValue);
            sut.NewValue.Should().Be(newValue);
        }
        
        [Fact]
        public void ThrowsExceptionWithNullMessageFormat()
        {
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new FormatArguments(null!, oldValue, newValue);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}