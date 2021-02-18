namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
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
        [InlineData("", PropertyModifiers.None)]
        [InlineData("new", PropertyModifiers.New)]
        [InlineData("new static", PropertyModifiers.NewStatic)]
        [InlineData("static new", PropertyModifiers.NewStatic)]
        [InlineData("new virtual", PropertyModifiers.NewVirtual)]
        [InlineData("virtual new", PropertyModifiers.NewVirtual)]
        [InlineData("new abstract", PropertyModifiers.NewAbstract)]
        [InlineData("abstract new", PropertyModifiers.NewAbstract)]
        [InlineData("new abstract virtual", PropertyModifiers.NewAbstractVirtual)]
        [InlineData("abstract new virtual", PropertyModifiers.NewAbstractVirtual)]
        [InlineData("abstract virtual new", PropertyModifiers.NewAbstractVirtual)]
        [InlineData("virtual abstract new", PropertyModifiers.NewAbstractVirtual)]
        [InlineData("virtual new abstract", PropertyModifiers.NewAbstractVirtual)]
        [InlineData("new virtual abstract", PropertyModifiers.NewAbstractVirtual)]
        [InlineData("abstract", PropertyModifiers.Abstract)]
        [InlineData("abstract override", PropertyModifiers.AbstractOverride)]
        [InlineData("override abstract", PropertyModifiers.AbstractOverride)]
        [InlineData("override", PropertyModifiers.Override)]
        [InlineData("override sealed", PropertyModifiers.SealedOverride)]
        [InlineData("sealed override", PropertyModifiers.SealedOverride)]
        [InlineData("sealed", PropertyModifiers.Sealed)]
        [InlineData("static", PropertyModifiers.Static)]
        [InlineData("virtual", PropertyModifiers.Virtual)]
        public async Task ModifiersReturnsExpectedValue(string modifiers, PropertyModifiers expected)
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