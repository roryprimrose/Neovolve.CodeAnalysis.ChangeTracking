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
        public async Task ArgumentTypeReturnsOrdinalForOrdinalArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.OrdinalArgument)
                .ConfigureAwait(false);

            var sut = new ArgumentDefinition(node, 1);

            sut.ArgumentType.Should().Be(ArgumentType.Ordinal);
        }

        [Fact]
        public async Task NameReturnsParameterNameForNamedArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.NamedArgument)
                .ConfigureAwait(false);

            var sut = new ArgumentDefinition(node, null);

            sut.Name.Should().Be("first");
        }

        [Fact]
        public async Task NameReturnsValueForOrdinalArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.OrdinalArgument)
                .ConfigureAwait(false);

            var sut = new ArgumentDefinition(node, 1);

            sut.Name.Should().Be("123");
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
        public async Task ParameterNameReturnsEmptyForOrdinalArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.OrdinalArgument)
                .ConfigureAwait(false);

            var sut = new ArgumentDefinition(node, 1);

            sut.ParameterName.Should().BeEmpty();
        }

        [Fact]
        public async Task ParameterNameReturnsNameForNamedArgument()
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