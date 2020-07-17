﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class PropertyDefinitionTests
    {
        [Fact]
        public async Task FullNameReturnsPropertyNameCombinedWithParentFullName()
        {
            var parentFullName = Guid.NewGuid().ToString();

            var declaringType = Substitute.For<IClassDefinition>();

            declaringType.FullName.Returns(parentFullName);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GetSetProperty)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.FullName.Should().Be(parentFullName + ".Value");
        }

        [Fact]
        public async Task FullRawNameReturnsPropertyNameCombinedWithParentFullRawName()
        {
            var parentFullRawName = Guid.NewGuid().ToString();

            var declaringType = Substitute.For<IClassDefinition>();

            declaringType.FullRawName.Returns(parentFullRawName);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GetSetProperty)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.FullRawName.Should().Be(parentFullRawName + ".Value");
        }

        [Fact]
        public async Task GetAccessorReturnsDefinitionForReadProperty()
        {
            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GetSetProperty)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.GetAccessor.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAccessorReturnsNullForReadOnlyProperty()
        {
            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.WriteOnlyProperty)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.GetAccessor.Should().BeNull();
        }

        [Theory]
        [InlineData("", MemberModifiers.None)]
        [InlineData("new", MemberModifiers.New)]
        [InlineData("new static", MemberModifiers.NewStatic)]
        [InlineData("static new", MemberModifiers.NewStatic)]
        [InlineData("new virtual", MemberModifiers.NewVirtual)]
        [InlineData("virtual new", MemberModifiers.NewVirtual)]
        [InlineData("new abstract", MemberModifiers.NewAbstract)]
        [InlineData("abstract new", MemberModifiers.NewAbstract)]
        [InlineData("new abstract virtual", MemberModifiers.NewAbstractVirtual)]
        [InlineData("abstract new virtual", MemberModifiers.NewAbstractVirtual)]
        [InlineData("abstract virtual new", MemberModifiers.NewAbstractVirtual)]
        [InlineData("virtual abstract new", MemberModifiers.NewAbstractVirtual)]
        [InlineData("virtual new abstract", MemberModifiers.NewAbstractVirtual)]
        [InlineData("new virtual abstract", MemberModifiers.NewAbstractVirtual)]
        [InlineData("abstract", MemberModifiers.Abstract)]
        [InlineData("abstract override", MemberModifiers.AbstractOverride)]
        [InlineData("override abstract", MemberModifiers.AbstractOverride)]
        [InlineData("override", MemberModifiers.Override)]
        [InlineData("override sealed", MemberModifiers.SealedOverride)]
        [InlineData("sealed override", MemberModifiers.SealedOverride)]
        [InlineData("sealed", MemberModifiers.Sealed)]
        [InlineData("static", MemberModifiers.Static)]
        [InlineData("virtual", MemberModifiers.Virtual)]
        public async Task ModifiersReturnsExpectedValue(string modifiers, MemberModifiers expected)
        {
            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode
                .FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.BuildPropertyWithModifiers(modifiers))
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.Modifiers.Should().Be(expected);
        }

        [Fact]
        public async Task NameReturnsPropertyName()
        {
            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GetSetProperty)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.Name.Should().Be("Value");
        }

        [Fact]
        public async Task RawNameReturnsPropertyName()
        {
            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GetSetProperty)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.RawName.Should().Be("Value");
        }

        [Fact]
        public async Task ReturnTypeReturnsGenericPropertyType()
        {
            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GenericProperty)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.ReturnType.Should().Be("T");
        }

        [Fact]
        public async Task ReturnTypeReturnsPropertyType()
        {
            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GetSetProperty)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.ReturnType.Should().Be("string");
        }

        [Fact]
        public async Task SetAccessorReturnsDefinitionForWriteProperty()
        {
            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GetSetProperty)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.SetAccessor.Should().NotBeNull();
        }

        [Fact]
        public async Task SetAccessorReturnsNullForReadOnlyProperty()
        {
            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.ReadOnlyProperty)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.SetAccessor.Should().BeNull();
        }

        [Fact]
        [SuppressMessage(
            "Usage",
            "CA1806:Do not ignore method results",
            Justification = "The constructor is the target of the test")]
        public async Task ThrowsExceptionWhenCreatedWithNullDeclaringType()
        {
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GetSetProperty)
                .ConfigureAwait(false);

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PropertyDefinition(null!, node);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage(
            "Usage",
            "CA1806:Do not ignore method results",
            Justification = "The constructor is the target of the test")]
        public void ThrowsExceptionWhenCreatedWithNullNode()
        {
            var declaringType = Substitute.For<IClassDefinition>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PropertyDefinition(declaringType, null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}