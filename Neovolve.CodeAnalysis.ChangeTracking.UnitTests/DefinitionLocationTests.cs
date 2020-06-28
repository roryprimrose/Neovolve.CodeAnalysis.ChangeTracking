namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class DefinitionLocationTests
    {
        [Fact]
        public void CanCreateWithEmptyFilePath()
        {
            var lineIndex = Math.Abs(Model.Create<int>());
            var characterIndex = Math.Abs(Model.Create<int>());

            var sut = new DefinitionLocation(string.Empty, lineIndex, characterIndex);

            sut.FilePath.Should().BeEmpty();
        }

        [Fact]
        public void FilePathTrimsProvidedValue()
        {
            var lineIndex = Math.Abs(Model.Create<int>());
            var characterIndex = Math.Abs(Model.Create<int>());
            var expected = Guid.NewGuid().ToString();
            var filePath = "  " + expected + "  ";

            var sut = new DefinitionLocation(filePath, lineIndex, characterIndex);

            sut.FilePath.Should().Be(expected);
        }

        [Fact]
        public void PropertiesReturnProvidedParameters()
        {
            var lineIndex = Math.Abs(Model.Create<int>());
            var characterIndex = Math.Abs(Model.Create<int>());
            var filePath = Guid.NewGuid().ToString();

            var sut = new DefinitionLocation(filePath, lineIndex, characterIndex);

            sut.FilePath.Should().Be(filePath);
            sut.LineIndex.Should().Be(lineIndex);
            sut.CharacterIndex.Should().Be(characterIndex);
        }

        [Fact]
        public void ThrowsExceptionWithNullFilePath()
        {
            var lineIndex = Math.Abs(Model.Create<int>());
            var characterIndex = Math.Abs(Model.Create<int>());

            Action action = () => new DefinitionLocation(null!, lineIndex, characterIndex);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}