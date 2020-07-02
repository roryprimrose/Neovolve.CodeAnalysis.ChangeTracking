namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class ArgumentDefinitionTests
    {
        [Fact]
        public async Task ArgumentTypeReturnsNamedForNamedArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.NamedArgument)
                .ConfigureAwait(false);

            var sut = new ArgumentDefinition(node, null);

            sut.ArgumentType.Should().Be(ArgumentType.Named);
        }

        [Fact]
        public async Task OrdinalIndexReturnsNullForNamedArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.NamedArgument)
                .ConfigureAwait(false);

            var sut = new ArgumentDefinition(node, 1);

            sut.OrdinalIndex.Should().NotHaveValue();
        }

        [Fact]
        public async Task OrdinalIndexReturnsParameterValueForOrdinalArgument()
        {
            var index = Environment.TickCount;

            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.OrdinalArgument)
                .ConfigureAwait(false);

            var sut = new ArgumentDefinition(node, index);

            sut.OrdinalIndex.Should().Be(index);
        }

        [Fact]
        public async Task ArgumentTypeReturnsOrdinalForOrdinalArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.OrdinalArgument)
                .ConfigureAwait(false);

            var sut = new ArgumentDefinition(node, 1);

            sut.ArgumentType.Should().Be(ArgumentType.Ordinal);
        }

        [Fact]
        public async Task LocationReturnsEmptyFilePathWhenNodeLacksSourceInformation()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.OrdinalArgument)
                .ConfigureAwait(false);

            var sut = new ArgumentDefinition(node, 1);

            sut.Location.FilePath.Should().BeEmpty();
        }

        [Fact]
        public async Task LocationReturnsFileContentLocation()
        {
            var filePath = Guid.NewGuid().ToString();

            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.OrdinalArgument, filePath)
                .ConfigureAwait(false);

            var sut = new ArgumentDefinition(node, 1);

            sut.Location.LineIndex.Should().Be(3);
            sut.Location.CharacterIndex.Should().Be(21);
        }

        [Fact]
        public async Task LocationReturnsFilePathWhenNodeIncludesSourceInformation()
        {
            var filePath = Guid.NewGuid().ToString();

            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.OrdinalArgument, filePath)
                .ConfigureAwait(false);

            var sut = new ArgumentDefinition(node, 1);

            sut.Location.FilePath.Should().Be(filePath);
        }

        [Fact]
        public async Task NameReturnsEmptyForOrdinalArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.OrdinalArgument)
                .ConfigureAwait(false);

            var sut = new ArgumentDefinition(node, 1);

            sut.Name.Should().BeEmpty();
        }

        [Fact]
        public async Task NameReturnsNameForNamedArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.NamedArgument)
                .ConfigureAwait(false);

            var sut = new ArgumentDefinition(node, null);

            sut.Name.Should().Be("first");
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullNode()
        {
            Action action = () => new ArgumentDefinition(null!, 1);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task ValueReturnsValueForNamedArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.NamedArgument)
                .ConfigureAwait(false);

            var sut = new ArgumentDefinition(node, 1);

            sut.Value.Should().Be("123");
        }

        [Fact]
        public async Task ValueReturnsValueForOrdinalArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.OrdinalArgument)
                .ConfigureAwait(false);

            var sut = new ArgumentDefinition(node, 1);

            sut.Value.Should().Be("123");
        }
    }
}