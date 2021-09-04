namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class ArgumentDefinitionTests
    {
        [Fact]
        public async Task ArgumentTypeReturnsNamedForNamedArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.NamedArgument)
                .ConfigureAwait(false);
            var attribute = new TestAttributeDefinition();

            var sut = new ArgumentDefinition(node, null, attribute);

            sut.ArgumentType.Should().Be(ArgumentType.Named);
        }

        [Fact]
        public async Task ArgumentTypeReturnsOrdinalForOrdinalArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.OrdinalArgument)
                .ConfigureAwait(false);
            var attribute = new TestAttributeDefinition();

            var sut = new ArgumentDefinition(node, 1, attribute);

            sut.ArgumentType.Should().Be(ArgumentType.Ordinal);
        }

        [Fact]
        public async Task DeclarationReturnsParameterNameAndValueForNamedArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.NamedArgument)
                .ConfigureAwait(false);
            var attribute = new TestAttributeDefinition();

            var sut = new ArgumentDefinition(node, 1, attribute);

            sut.Declaration.Should().Be("first: 123");
        }

        [Fact]
        public async Task DeclarationReturnsValueForOrdinalArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.OrdinalArgument)
                .ConfigureAwait(false);
            var attribute = new TestAttributeDefinition();

            var sut = new ArgumentDefinition(node, 1, attribute);

            sut.Declaration.Should().Be("123");
        }

        [Fact]
        public async Task DeclaringAttributeReturnsConstructorValue()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.NamedArgument)
                .ConfigureAwait(false);
            var attribute = new TestAttributeDefinition();

            var sut = new ArgumentDefinition(node, null, attribute);

            sut.DeclaringAttribute.Should().Be(attribute);
        }

        [Fact]
        public async Task NameReturnsParameterNameForNamedArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.NamedArgument)
                .ConfigureAwait(false);
            var attribute = new TestAttributeDefinition();

            var sut = new ArgumentDefinition(node, 1, attribute);

            sut.Name.Should().Be("first");
        }

        [Fact]
        public async Task NameReturnsValueForOrdinalArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.OrdinalArgument)
                .ConfigureAwait(false);
            var attribute = new TestAttributeDefinition();

            var sut = new ArgumentDefinition(node, 1, attribute);

            sut.Name.Should().Be("123");
        }

        [Fact]
        public async Task OrdinalIndexReturnsNullForNamedArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.NamedArgument)
                .ConfigureAwait(false);
            var attribute = new TestAttributeDefinition();

            var sut = new ArgumentDefinition(node, 1, attribute);

            sut.OrdinalIndex.Should().NotHaveValue();
        }

        [Fact]
        public async Task OrdinalIndexReturnsParameterValueForOrdinalArgument()
        {
            var index = Environment.TickCount;

            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.OrdinalArgument)
                .ConfigureAwait(false);
            var attribute = new TestAttributeDefinition();

            var sut = new ArgumentDefinition(node, index, attribute);

            sut.OrdinalIndex.Should().Be(index);
        }

        [Fact]
        public async Task ParameterNameReturnsEmptyForOrdinalArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.OrdinalArgument)
                .ConfigureAwait(false);
            var attribute = new TestAttributeDefinition();

            var sut = new ArgumentDefinition(node, 1, attribute);

            sut.ParameterName.Should().BeEmpty();
        }

        [Fact]
        public async Task ParameterNameReturnsNameForNamedArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.NamedArgument)
                .ConfigureAwait(false);
            var attribute = new TestAttributeDefinition();

            var sut = new ArgumentDefinition(node, 1, attribute);

            sut.Name.Should().Be("first");
        }

        [Fact]
        public async Task ThrowsExceptionWhenCreatedWithNullAttribute()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.NamedArgument)
                .ConfigureAwait(false);

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ArgumentDefinition(node, 1, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullNode()
        {
            var attribute = new TestAttributeDefinition();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ArgumentDefinition(null!, 1, attribute);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task ValueReturnsValueForNamedArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.NamedArgument)
                .ConfigureAwait(false);
            var attribute = new TestAttributeDefinition();

            var sut = new ArgumentDefinition(node, 1, attribute);

            sut.Value.Should().Be("123");
        }

        [Fact]
        public async Task ValueReturnsValueForOrdinalArgument()
        {
            var node = await TestNode.FindNode<AttributeArgumentSyntax>(ArgumentDefinitionCode.OrdinalArgument)
                .ConfigureAwait(false);
            var attribute = new TestAttributeDefinition();

            var sut = new ArgumentDefinition(node, 1, attribute);

            sut.Value.Should().Be("123");
        }
    }
}