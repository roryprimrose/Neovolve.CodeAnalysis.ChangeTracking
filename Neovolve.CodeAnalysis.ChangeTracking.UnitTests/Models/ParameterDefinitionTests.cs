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
            var declaringMethod = Substitute.For<IMethodDefinition>();

            var node = await TestNode
                .FindNode<ParameterSyntax>(ParameterDefinitionCode.SingleParameter.Replace("string value",
                    "[Tagger(123, true, \"abc\")]string value"))
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMethod, node);

            sut.Attributes.Should().HaveCount(1);
            sut.Attributes.First().Name.Should().Be("Tagger");
            sut.Attributes.First().Arguments.Should().HaveCount(3);
        }

        [Fact]
        public async Task DeclaringMemberReturnsProvidedValue()
        {
            var declaringMethod = Substitute.For<IMethodDefinition>();

            var node = await TestNode.FindNode<ParameterSyntax>(ParameterDefinitionCode.SingleParameter)
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMethod, node);

            sut.DeclaringMember.Should().Be(declaringMethod);
        }

        [Theory]
        [InlineData("")]
        [InlineData("\"123\"")]
        [InlineData("123")]
        [InlineData("true")]
        public async Task DefaultValueReturnsDeclaredValue(string defaultValue)
        {
            var declaringMethod = Substitute.For<IMethodDefinition>();

            var code = ParameterDefinitionCode.SingleParameter;

            if (string.IsNullOrWhiteSpace(defaultValue) == false)
            {
                code = code.Replace("value", "value = " + defaultValue);
            }

            var node = await TestNode
                .FindNode<ParameterSyntax>(code)
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMethod, node);

            sut.DefaultValue.Should().Be(defaultValue);
        }

        [Fact]
        public async Task FullNameReturnsDeclaredValueWithDeclaringMemberFullName()
        {
            string fullName = Guid.NewGuid().ToString();

            var declaringMethod = Substitute.For<IMethodDefinition>();

            declaringMethod.FullName.Returns(fullName);

            var node = await TestNode.FindNode<ParameterSyntax>(ParameterDefinitionCode.SingleParameter)
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMethod, node);

            sut.FullName.Should().Be(fullName + "_value");
        }

        [Fact]
        public async Task FullRawNameReturnsDeclaredValueWithDeclaringMemberFullName()
        {
            string fullRawName = Guid.NewGuid().ToString();

            var declaringMethod = Substitute.For<IMethodDefinition>();

            declaringMethod.FullRawName.Returns(fullRawName);

            var node = await TestNode.FindNode<ParameterSyntax>(ParameterDefinitionCode.SingleParameter)
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMethod, node);

            sut.FullRawName.Should().Be(fullRawName + "_value");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task IsVisibleReturnsDeclaringMemberIsVisible(bool value)
        {
            var declaringMethod = Substitute.For<IMethodDefinition>();

            declaringMethod.IsVisible.Returns(value);

            var node = await TestNode.FindNode<ParameterSyntax>(ParameterDefinitionCode.SingleParameter)
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMethod, node);

            sut.IsVisible.Should().Be(value);
        }

        [Theory]
        [InlineData("", ParameterModifiers.None)]
        [InlineData("ref", ParameterModifiers.Ref)]
        [InlineData("out", ParameterModifiers.Out)]
        [InlineData("params", ParameterModifiers.Params)]
        [InlineData("this", ParameterModifiers.This)]
        public async Task ModifiersReturnsDeclaredValues(string modifiers, ParameterModifiers expected)
        {
            var declaringMethod = Substitute.For<IMethodDefinition>();

            var node = await TestNode
                .FindNode<ParameterSyntax>(
                    ParameterDefinitionCode.SingleParameter.Replace("string value", modifiers + " string value"))
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMethod, node);

            sut.DeclaredModifiers.Should().Be(modifiers);
            sut.Modifiers.Should().Be(expected);
        }

        [Fact]
        public async Task NameReturnsDeclaredValue()
        {
            var declaringMethod = Substitute.For<IMethodDefinition>();

            var node = await TestNode.FindNode<ParameterSyntax>(ParameterDefinitionCode.SingleParameter)
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMethod, node);

            sut.Name.Should().Be("value");
        }

        [Fact]
        public async Task RawNameReturnsDeclaredValue()
        {
            var declaringMethod = Substitute.For<IMethodDefinition>();

            var node = await TestNode.FindNode<ParameterSyntax>(ParameterDefinitionCode.SingleParameter)
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMethod, node);

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
            var declaringMethod = Substitute.For<IMethodDefinition>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ParameterDefinition(declaringMethod, null!);

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
            var declaringMethod = Substitute.For<IMethodDefinition>();

            var node = await TestNode
                .FindNode<ParameterSyntax>(ParameterDefinitionCode.SingleParameter.Replace("string", typeName))
                .ConfigureAwait(false);

            var sut = new ParameterDefinition(declaringMethod, node);

            sut.Type.Should().Be(typeName);
        }
    }
}