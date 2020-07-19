namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class ComparerOptionsTests
    {
        [Fact]
        public void CanSetNewMessageFormatter()
        {
            var messageFormatter = Substitute.For<IMessageFormatter>();

            var sut = new ComparerOptions {MessageFormatter = messageFormatter};

            sut.MessageFormatter.Should().Be(messageFormatter);
        }

        [Fact]
        public void CanSetSkipAttributes()
        {
            var sut = new ComparerOptions {SkipAttributes = true};

            sut.SkipAttributes.Should().BeTrue();
        }

        [Fact]
        public void DefaultReturnsDefaultOptions()
        {
            var sut = ComparerOptions.Default;

            sut.MessageFormatter.Should().BeOfType<DefaultMessageFormatter>();
            sut.SkipAttributes.Should().BeFalse();
        }

        [Fact]
        public void DefaultReturnsNewInstanceOnEachCall()
        {
            var first = ComparerOptions.Default;
            var second = ComparerOptions.Default;

            first.Should().NotBeSameAs(second);
        }
    }
}