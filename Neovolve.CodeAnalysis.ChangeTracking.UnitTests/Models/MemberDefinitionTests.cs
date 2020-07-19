namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class MemberDefinitionTests
    {
        [Fact]
        public async Task AccessModifierReturnsPrivateWhenEmptyModifierDefinedWithClassParent()
        {
            var code = FieldDefinitionCode.BuildFieldWithModifiers(string.Empty);

            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new Wrapper(declaringType, node);

            sut.AccessModifier.Should().Be(AccessModifier.Private);
        }

        [Fact]
        public async Task AccessModifierReturnsPublicWhenEmptyModifierDefinedWithInterfaceParent()
        {
            var code = FieldDefinitionCode.BuildFieldWithModifiers(string.Empty);

            var declaringType = Substitute.For<IInterfaceDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new Wrapper(declaringType, node);

            sut.AccessModifier.Should().Be(AccessModifier.Public);
        }

        [Theory]
        [InlineData("private", AccessModifier.Private)]
        [InlineData("internal", AccessModifier.Internal)]
        [InlineData("protected", AccessModifier.Protected)]
        [InlineData("private protected", AccessModifier.ProtectedPrivate)]
        [InlineData("protected private", AccessModifier.ProtectedPrivate)]
        [InlineData("protected internal", AccessModifier.ProtectedInternal)]
        [InlineData("internal protected", AccessModifier.ProtectedInternal)]
        [InlineData("public", AccessModifier.Public)]
        public async Task AccessModifierReturnsValueBasedOnAccessModifiers(
            string accessModifiers,
            AccessModifier expected)
        {
            var code = FieldDefinitionCode.BuildFieldWithModifiers(accessModifiers);

            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new Wrapper(declaringType, node);

            sut.AccessModifier.Should().Be(expected);
        }

        [Fact]
        public async Task DeclaringTypeReturnsParameterValue()
        {
            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(FieldDefinitionCode.SingleField)
                .ConfigureAwait(false);

            var sut = new Wrapper(declaringType, node);

            sut.DeclaringType.Should().Be(declaringType);
        }

        [Theory]
        [InlineData(true, "private", false)]
        [InlineData(true, "internal", false)]
        [InlineData(true, "protected", true)]
        [InlineData(true, "private protected", true)]
        [InlineData(true, "protected private", true)]
        [InlineData(true, "protected internal", true)]
        [InlineData(true, "internal protected", true)]
        [InlineData(true, "public", true)]
        [InlineData(false, "private", false)]
        [InlineData(false, "internal", false)]
        [InlineData(false, "protected", false)]
        [InlineData(false, "private protected", false)]
        [InlineData(false, "protected private", false)]
        [InlineData(false, "protected internal", false)]
        [InlineData(false, "internal protected", false)]
        [InlineData(false, "public", false)]
        public async Task IsVisibleReturnsWhetherParentAndAccessModifierAreVisible(bool parentIsVisible,
            string accessModifiers, bool expected)
        {
            var code = FieldDefinitionCode.BuildFieldWithModifiers(accessModifiers);

            var declaringType = Substitute.For<IClassDefinition>();

            declaringType.IsVisible.Returns(parentIsVisible);

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new Wrapper(declaringType, node);

            sut.IsVisible.Should().Be(expected);
        }

        [Fact]
        public async Task ThrowsExceptionWhenDeclaringTypeIsNotSupported()
        {
            var code = FieldDefinitionCode.BuildFieldWithModifiers(string.Empty);

            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Wrapper(declaringType, node);

            action.Should().Throw<NotSupportedException>();
        }

        private class Wrapper : MemberDefinition
        {
            public Wrapper(ITypeDefinition declaringType, MemberDeclarationSyntax node) : base(node, declaringType)
            {
            }

            public override string FullName { get; } = string.Empty;
            public override string FullRawName { get; } = string.Empty;
            public override string Name { get; } = string.Empty;
            public override string RawName { get; } = string.Empty;
            public override string ReturnType { get; } = string.Empty;
        }
    }
}