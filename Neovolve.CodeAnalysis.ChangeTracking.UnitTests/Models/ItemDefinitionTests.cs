namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class ItemDefinitionTests
    {
        [Fact]
        public async Task LocationReturnsEmptyFilePathWhenNodeLacksSourceInformation()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var sut = new Wrapper(node);

            sut.Location.FilePath.Should().BeEmpty();
        }

        [Fact]
        public async Task LocationReturnsFileContentLocation()
        {
            var filePath = Guid.NewGuid().ToString();

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent, filePath)
                .ConfigureAwait(false);

            var sut = new Wrapper(node);

            sut.Location.LineIndex.Should().Be(3);
            sut.Location.CharacterIndex.Should().Be(4);
        }

        [Fact]
        public async Task LocationReturnsFilePathWhenNodeIncludesSourceInformation()
        {
            var filePath = Guid.NewGuid().ToString();

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent, filePath)
                .ConfigureAwait(false);

            var sut = new Wrapper(node);

            sut.Location.FilePath.Should().Be(filePath);
        }

        [Fact]
        [SuppressMessage(
            "Usage",
            "CA1806:Do not ignore method results",
            Justification = "The constructor is the target of the test")]
        public void ThrowsExceptionWhenCreatedWithNullNode()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Wrapper(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : ItemDefinition
        {
            public Wrapper(MemberDeclarationSyntax node) : base(node)
            {
                Description = Guid.NewGuid().ToString();
                Name = Guid.NewGuid().ToString();
            }

            public override string Description { get; }
            public override string Name { get; }
        }
    }
}