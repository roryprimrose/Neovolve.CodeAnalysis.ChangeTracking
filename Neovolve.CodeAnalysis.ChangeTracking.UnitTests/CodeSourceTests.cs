namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class CodeSourceTests
    {
        [Fact]
        public void CanCreateWithContents()
        {
            var contents = Guid.NewGuid().ToString();

            var actual = new CodeSource(contents);

            actual.Contents.Should().Be(contents);
            actual.FilePath.Should().BeEmpty();
        }

        [Fact]
        public void CanCreateWithFilePathAndContents()
        {
            var filePath = Guid.NewGuid().ToString();
            var contents = Guid.NewGuid().ToString();

            var actual = new CodeSource(filePath, contents);

            actual.Contents.Should().Be(contents);
            actual.FilePath.Should().Be(filePath);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void ThrowsExceptionWhenCreatedWithInvalidFilePath(string filePath)
        {
            var contents = Guid.NewGuid().ToString();

            Action action = () => new CodeSource(filePath, contents);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullContents()
        {
            var filePath = Guid.NewGuid().ToString();

            Action action = () => new CodeSource(filePath, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullContentsAndNoFilePath()
        {
            Action action = () => new CodeSource(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}