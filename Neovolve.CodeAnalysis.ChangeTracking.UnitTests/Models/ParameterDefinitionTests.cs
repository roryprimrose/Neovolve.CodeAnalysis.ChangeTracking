namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class ParameterDefinitionTests
    {
        [Fact]
        public async Task AttributesReturnsDeclaredValues()
        {
            var declaringMember = Substitute.For<IMemberDefinition>();

            var node = await TestNode
                .FindNode<ParameterSyntax>(ParameterDefinitionCode.SingleParameter.Replace("string value",
                    "[Tagger(123, true, \"abc\")]string value"))
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMember, node);

            sut.Attributes.Should().HaveCount(1);
            sut.Attributes.First().Name.Should().Be("Tagger");
            sut.Attributes.First().Arguments.Should().HaveCount(3);
        }

        [Fact]
        public async Task DeclaringMemberReturnsProvidedValue()
        {
            var declaringMember = Substitute.For<IMemberDefinition>();

            var node = await TestNode.FindNode<ParameterSyntax>(ParameterDefinitionCode.SingleParameter)
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMember, node);

            sut.DeclaringMember.Should().Be(declaringMember);
        }

        [Theory]
        [InlineData("")]
        [InlineData("\"123\"")]
        [InlineData("123")]
        [InlineData("true")]
        public async Task DefaultValueReturnsDeclaredValue(string defaultValue)
        {
            var declaringMember = Substitute.For<IMemberDefinition>();

            var code = ParameterDefinitionCode.SingleParameter;

            if (string.IsNullOrWhiteSpace(defaultValue) == false)
            {
                code = code.Replace("value", "value = " + defaultValue);
            }

            var node = await TestNode
                .FindNode<ParameterSyntax>(code)
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMember, node);

            sut.DefaultValue.Should().Be(defaultValue);
        }

        [Fact]
        public async Task FullNameReturnsDeclaredValueWithDeclaringMemberFullName()
        {
            string fullName = Guid.NewGuid().ToString();

            var declaringMember = Substitute.For<IMemberDefinition>();

            declaringMember.FullName.Returns(fullName);

            var node = await TestNode.FindNode<ParameterSyntax>(ParameterDefinitionCode.SingleParameter)
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMember, node);

            sut.FullName.Should().Be(fullName + "_value");
        }

        [Fact]
        public async Task FullRawNameReturnsDeclaredValueWithDeclaringMemberFullName()
        {
            string fullRawName = Guid.NewGuid().ToString();

            var declaringMember = Substitute.For<IMemberDefinition>();

            declaringMember.FullRawName.Returns(fullRawName);

            var node = await TestNode.FindNode<ParameterSyntax>(ParameterDefinitionCode.SingleParameter)
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMember, node);

            sut.FullRawName.Should().Be(fullRawName + "_value");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task IsVisibleReturnsDeclaringMemberIsVisible(bool value)
        {
            var declaringMember = Substitute.For<IMemberDefinition>();

            declaringMember.IsVisible.Returns(value);

            var node = await TestNode.FindNode<ParameterSyntax>(ParameterDefinitionCode.SingleParameter)
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMember, node);

            sut.IsVisible.Should().Be(value);
        }

        [Theory]
        [InlineData("", ParameterModifier.None)]
        [InlineData("ref", ParameterModifier.Ref)]
        [InlineData("out", ParameterModifier.Out)]
        [InlineData("params", ParameterModifier.Params)]
        [InlineData("this", ParameterModifier.This)]
        public async Task ModifiersReturnsDeclaredValues(string modifiers, ParameterModifier expected)
        {
            var declaringMember = Substitute.For<IMemberDefinition>();

            var node = await TestNode
                .FindNode<ParameterSyntax>(
                    ParameterDefinitionCode.SingleParameter.Replace("string value", modifiers + " string value"))
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMember, node);

            sut.DeclaredModifiers.Should().Be(modifiers);
            sut.Modifier.Should().Be(expected);
        }

        [Fact]
        public async Task NameReturnsDeclaredValue()
        {
            var declaringMember = Substitute.For<IMemberDefinition>();

            var node = await TestNode.FindNode<ParameterSyntax>(ParameterDefinitionCode.SingleParameter)
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMember, node);

            sut.Name.Should().Be("value");
        }

        [Fact]
        public async Task RawNameReturnsDeclaredValue()
        {
            var declaringMember = Substitute.For<IMemberDefinition>();

            var node = await TestNode.FindNode<ParameterSyntax>(ParameterDefinitionCode.SingleParameter)
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMember, node);

            sut.RawName.Should().Be("value");
        }

        [Fact]
        public async Task ThrowsExceptionWithNullDeclaringMember()
        {
            var node = await TestNode.FindNode<ParameterSyntax>(ParameterDefinitionCode.SingleParameter)
                .ConfigureAwait(false);

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ParameterDefinition(null!, node);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullNode()
        {
            var declaringMember = Substitute.For<IMemberDefinition>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ParameterDefinition(declaringMember, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("string")]
        [InlineData("string?")]
        [InlineData("Task<string>")]
        [InlineData("System.String")]
        [InlineData("dynamic")]
        public async Task TypeReturnsParameterType(string typeName)
        {
            var declaringMember = Substitute.For<IMemberDefinition>();

            var node = await TestNode
                .FindNode<ParameterSyntax>(ParameterDefinitionCode.SingleParameter.Replace("string", typeName))
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMember, node);

            sut.Type.Should().Be(typeName);
        }
    }
}