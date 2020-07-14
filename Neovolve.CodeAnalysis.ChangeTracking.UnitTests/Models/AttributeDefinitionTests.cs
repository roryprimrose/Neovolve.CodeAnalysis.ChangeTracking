namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class AttributeDefinitionTests
    {
        [Theory]
        [InlineData(AttributeDefinitionCode.SimpleAttribute)]
        [InlineData(AttributeDefinitionCode.SimpleAttributeWithBrackets)]
        public async Task ArgumentsReturnsEmptyWhenNoParametersDefined(string code)
        {
            var declaringItem = Substitute.For<IMemberDefinition>();

            var node = await TestNode.FindNode<AttributeSyntax>(code)
                .ConfigureAwait(false);

            var sut = new AttributeDefinition(declaringItem, node);

            sut.Arguments.Should().BeEmpty();
        }

        [Fact]
        public async Task ArgumentsReturnsMixedOrdinalAndNamedArguments()
        {
            var declaringItem = Substitute.For<IMemberDefinition>();

            var node = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.AttributeWithMixedOrdinalAndNamedArguments)
                .ConfigureAwait(false);

            var sut = new AttributeDefinition(declaringItem, node);

            sut.Arguments.Should().HaveCount(4);

            var firstArgument = sut.Arguments.First();

            firstArgument.Value.Should().Be("\"stringValue\"");
            firstArgument.OrdinalIndex.Should().Be(0);
            firstArgument.Name.Should().BeEmpty();
            firstArgument.ArgumentType.Should().Be(ArgumentType.Ordinal);

            var secondArgument = sut.Arguments.Skip(1).First();

            secondArgument.Value.Should().Be("123");
            secondArgument.OrdinalIndex.Should().Be(1);
            secondArgument.Name.Should().BeEmpty();
            secondArgument.ArgumentType.Should().Be(ArgumentType.Ordinal);

            var thirdArgument = sut.Arguments.Skip(2).First();

            thirdArgument.Value.Should().Be("true");
            thirdArgument.OrdinalIndex.Should().NotHaveValue();
            thirdArgument.Name.Should().Be("first");
            thirdArgument.ArgumentType.Should().Be(ArgumentType.Named);

            var fourthArgument = sut.Arguments.Skip(3).First();

            fourthArgument.Value.Should().Be("SomeConstant");
            fourthArgument.OrdinalIndex.Should().NotHaveValue();
            fourthArgument.Name.Should().Be("second");
            fourthArgument.ArgumentType.Should().Be(ArgumentType.Named);
        }

        [Fact]
        public async Task ArgumentsReturnsNamedArguments()
        {
            var declaringItem = Substitute.For<IMemberDefinition>();

            var node = await TestNode.FindNode<AttributeSyntax>(AttributeDefinitionCode.AttributeWithNamedArguments)
                .ConfigureAwait(false);

            var sut = new AttributeDefinition(declaringItem, node);

            sut.Arguments.Should().HaveCount(3);

            var firstArgument = sut.Arguments.First();

            firstArgument.Value.Should().Be("\"stringValue\"");
            firstArgument.OrdinalIndex.Should().NotHaveValue();
            firstArgument.Name.Should().Be("first");
            firstArgument.ArgumentType.Should().Be(ArgumentType.Named);

            var secondArgument = sut.Arguments.Skip(1).First();

            secondArgument.Value.Should().Be("123");
            secondArgument.OrdinalIndex.Should().NotHaveValue();
            secondArgument.Name.Should().Be("second");
            secondArgument.ArgumentType.Should().Be(ArgumentType.Named);

            var thirdArgument = sut.Arguments.Skip(2).First();

            thirdArgument.Value.Should().Be("true");
            thirdArgument.OrdinalIndex.Should().NotHaveValue();
            thirdArgument.Name.Should().Be("third");
            thirdArgument.ArgumentType.Should().Be(ArgumentType.Named);
        }

        [Fact]
        public async Task ArgumentsReturnsOrdinalArguments()
        {
            var declaringItem = Substitute.For<IMemberDefinition>();

            var node = await TestNode.FindNode<AttributeSyntax>(AttributeDefinitionCode.AttributeWithOrdinalArguments)
                .ConfigureAwait(false);

            var sut = new AttributeDefinition(declaringItem, node);

            sut.Arguments.Should().HaveCount(3);

            var firstArgument = sut.Arguments.First();

            firstArgument.Value.Should().Be("\"stringValue\"");
            firstArgument.OrdinalIndex.Should().Be(0);
            firstArgument.Name.Should().BeEmpty();
            firstArgument.ArgumentType.Should().Be(ArgumentType.Ordinal);

            var secondArgument = sut.Arguments.Skip(1).First();

            secondArgument.Value.Should().Be("123");
            secondArgument.OrdinalIndex.Should().Be(1);
            secondArgument.Name.Should().BeEmpty();
            secondArgument.ArgumentType.Should().Be(ArgumentType.Ordinal);

            var thirdArgument = sut.Arguments.Skip(2).First();

            thirdArgument.Value.Should().Be("true");
            thirdArgument.OrdinalIndex.Should().Be(2);
            thirdArgument.Name.Should().BeEmpty();
            thirdArgument.ArgumentType.Should().Be(ArgumentType.Ordinal);
        }

        [Fact]
        public async Task DeclaredOnReturnsParameterValue()
        {
            var declaringItem = Substitute.For<IMemberDefinition>();

            var node = await TestNode.FindNode<AttributeSyntax>(AttributeDefinitionCode.SimpleAttributeWithBrackets)
                .ConfigureAwait(false);

            var sut = new AttributeDefinition(declaringItem, node);

            sut.DeclaredOn.Should().Be(declaringItem);
        }

        [Fact]
        public async Task LocationReturnsEmptyFilePathWhenNodeLacksSourceInformation()
        {
            var declaringItem = Substitute.For<IMemberDefinition>();

            var node = await TestNode.FindNode<AttributeSyntax>(AttributeDefinitionCode.SimpleAttribute)
                .ConfigureAwait(false);

            var sut = new AttributeDefinition(declaringItem, node);

            sut.Location.FilePath.Should().BeEmpty();
        }

        [Fact]
        public async Task LocationReturnsFileContentLocation()
        {
            var filePath = Guid.NewGuid().ToString();

            var declaringItem = Substitute.For<IMemberDefinition>();

            var node = await TestNode.FindNode<AttributeSyntax>(AttributeDefinitionCode.SimpleAttribute, filePath)
                .ConfigureAwait(false);

            var sut = new AttributeDefinition(declaringItem, node);

            sut.Location.LineIndex.Should().Be(3);
            sut.Location.CharacterIndex.Should().Be(5);
        }

        [Fact]
        public async Task LocationReturnsFilePathWhenNodeIncludesSourceInformation()
        {
            var filePath = Guid.NewGuid().ToString();

            var declaringItem = Substitute.For<IMemberDefinition>();

            var node = await TestNode.FindNode<AttributeSyntax>(AttributeDefinitionCode.SimpleAttribute, filePath)
                .ConfigureAwait(false);

            var sut = new AttributeDefinition(declaringItem, node);

            sut.Location.FilePath.Should().Be(filePath);
        }

        [Fact]
        public async Task NameReturnsNameFromAttribute()
        {
            var declaringItem = Substitute.For<IMemberDefinition>();

            var node = await TestNode.FindNode<AttributeSyntax>(AttributeDefinitionCode.SimpleAttribute)
                .ConfigureAwait(false);

            var sut = new AttributeDefinition(declaringItem, node);

            sut.Name.Should().Be("SimpleAttribute");
        }

        [Fact]
        public async Task NameReturnsNameFromAttributeWithArguments()
        {
            var declaringItem = Substitute.For<IMemberDefinition>();

            var node = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.AttributeWithMixedOrdinalAndNamedArguments)
                .ConfigureAwait(false);

            var sut = new AttributeDefinition(declaringItem, node);

            sut.Name.Should().Be("SimpleAttribute");
        }

        [Fact]
        public async Task NameReturnsNameFromAttributeWithBrackets()
        {
            var declaringItem = Substitute.For<IMemberDefinition>();

            var node = await TestNode.FindNode<AttributeSyntax>(AttributeDefinitionCode.SimpleAttributeWithBrackets)
                .ConfigureAwait(false);

            var sut = new AttributeDefinition(declaringItem, node);

            sut.Name.Should().Be("SimpleAttribute");
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification =
            "The constructor is the target of the test")]
        public async Task ThrowsExceptionWhenCreatedWithNullDeclaringItem()
        {
            var node = await TestNode.FindNode<AttributeSyntax>(AttributeDefinitionCode.SimpleAttributeWithBrackets)
                .ConfigureAwait(false);

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new AttributeDefinition(null!, node);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification =
            "The constructor is the target of the test")]
        public void ThrowsExceptionWhenCreatedWithNullNode()
        {
            var declaringItem = Substitute.For<IMemberDefinition>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new AttributeDefinition(declaringItem, null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}