namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class FormatArgumentsTests
    {
        [Fact]
        public void PropertiesReturnConstructorParameters()
        {
            var messageFormat = Guid.NewGuid().ToString();
            var identifier = Guid.NewGuid().ToString();
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();

            var sut = new FormatArguments(messageFormat, identifier, oldValue, newValue);

            sut.MessageFormat.Should().Be(messageFormat);
            sut.Identifier.Should().Be(identifier);
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
            Action action = () => new FormatArguments(messageFormat, null!, oldValue, newValue);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullMessageFormat()
        {
            var identifier = Guid.NewGuid().ToString();
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new FormatArguments(null!, identifier, oldValue, newValue);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}