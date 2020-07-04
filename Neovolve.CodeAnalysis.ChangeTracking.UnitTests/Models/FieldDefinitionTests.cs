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

    public class FieldDefinitionTests
    {
        [Fact]
        public async Task FullNameReturnsFieldNameCombinedWithParentFullName()
        {
            var parentFullName = Guid.NewGuid().ToString();

            var declaringType = Substitute.For<IClassDefinition>();

            declaringType.FullName.Returns(parentFullName);

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(FieldDefinitionCode.GetSetField)
                .ConfigureAwait(false);

            var sut = new FieldDefinition(declaringType, node);

            sut.FullName.Should().Be(parentFullName + ".Value");
        }

        [Fact]
        public async Task FullRawNameReturnsFieldNameCombinedWithParentFullRawName()
        {
            var parentFullRawName = Guid.NewGuid().ToString();

            var declaringType = Substitute.For<IClassDefinition>();

            declaringType.FullRawName.Returns(parentFullRawName);

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(FieldDefinitionCode.GetSetField)
                .ConfigureAwait(false);

            var sut = new FieldDefinition(declaringType, node);

            sut.FullRawName.Should().Be(parentFullRawName + ".Value");
        }

        [Fact]
        public async Task NameReturnsFieldName()
        {
            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(FieldDefinitionCode.GetSetField)
                .ConfigureAwait(false);

            var sut = new FieldDefinition(declaringType, node);

            sut.Name.Should().Be("Value");
        }

        [Fact]
        public async Task RawNameReturnsFieldName()
        {
            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(FieldDefinitionCode.GetSetField)
                .ConfigureAwait(false);

            var sut = new FieldDefinition(declaringType, node);

            sut.RawName.Should().Be("Value");
        }

        [Fact]
        public async Task ReturnTypeReturnsFieldType()
        {
            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(FieldDefinitionCode.GetSetField)
                .ConfigureAwait(false);

            var sut = new FieldDefinition(declaringType, node);

            sut.ReturnType.Should().Be("string");
        }

        [Fact]
        public async Task ReturnTypeReturnsGenericFieldType()
        {
            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(FieldDefinitionCode.GenericField)
                .ConfigureAwait(false);

            var sut = new FieldDefinition(declaringType, node);

            sut.ReturnType.Should().Be("T");
        }

        [Fact]
        [SuppressMessage(
            "Usage",
            "CA1806:Do not ignore method results",
            Justification = "The constructor is the target of the test")]
        public async Task ThrowsExceptionWhenCreatedWithNullDeclaringType()
        {
            var node = await TestNode.FindNode<FieldDeclarationSyntax>(FieldDefinitionCode.GetSetField)
                .ConfigureAwait(false);

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new FieldDefinition(null!, node);

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
            Action action = () => new FieldDefinition(declaringType, null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}