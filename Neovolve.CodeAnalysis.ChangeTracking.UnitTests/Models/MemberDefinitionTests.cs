﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
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
            var code = FieldDefinitionCode.BuildClassFieldWithModifiers(string.Empty);

            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new Wrapper(declaringType, node);

            sut.AccessModifiers.Should().Be(AccessModifiers.Private);
        }

        [Fact]
        public async Task AccessModifierReturnsPrivateWhenEmptyModifierDefinedWithStructParent()
        {
            var code = FieldDefinitionCode.BuildStructFieldWithModifiers(string.Empty);

            var declaringType = Substitute.For<IStructDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new Wrapper(declaringType, node);

            sut.AccessModifiers.Should().Be(AccessModifiers.Private);
        }

        [Fact]
        public async Task AccessModifierReturnsPublicWhenEmptyModifierDefinedWithInterfaceParent()
        {
            var code = PropertyDefinitionCode.BuildInterfacePropertyWithModifiers(string.Empty);

            var declaringType = Substitute.For<IInterfaceDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new Wrapper(declaringType, node);

            sut.AccessModifiers.Should().Be(AccessModifiers.Public);
        }

        [Theory]
        [InlineData("private", AccessModifiers.Private)]
        [InlineData("internal", AccessModifiers.Internal)]
        [InlineData("protected", AccessModifiers.Protected)]
        [InlineData("private protected", AccessModifiers.ProtectedPrivate)]
        [InlineData("protected private", AccessModifiers.ProtectedPrivate)]
        [InlineData("protected internal", AccessModifiers.ProtectedInternal)]
        [InlineData("internal protected", AccessModifiers.ProtectedInternal)]
        [InlineData("public", AccessModifiers.Public)]
        public async Task AccessModifierReturnsValueBasedOnAccessModifiers(
            string accessModifiers,
            AccessModifiers expected)
        {
            var code = FieldDefinitionCode.BuildClassFieldWithModifiers(accessModifiers);

            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new Wrapper(declaringType, node);

            sut.AccessModifiers.Should().Be(expected);
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
            var code = FieldDefinitionCode.BuildClassFieldWithModifiers(accessModifiers);

            var declaringType = Substitute.For<IClassDefinition>();

            declaringType.IsVisible.Returns(parentIsVisible);

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new Wrapper(declaringType, node);

            sut.IsVisible.Should().Be(expected);
        }

        [Fact]
        public async Task ThrowsExceptionWhenDeclaringTypeIsNotSupported()
        {
            var code = FieldDefinitionCode.BuildClassFieldWithModifiers(string.Empty);

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