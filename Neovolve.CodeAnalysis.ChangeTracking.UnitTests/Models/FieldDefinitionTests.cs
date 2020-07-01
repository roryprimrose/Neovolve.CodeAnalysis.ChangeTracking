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

    public class FieldDefinitionTests
    {
        [Fact]
        public async Task AttributesReturnsMultipleAttributesOnMultipleLists()
        {
            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode
                .FindNode<FieldDeclarationSyntax>(FieldDefinitionCode
                    .FieldWithMultipleAttributesInMultipleLists)
                .ConfigureAwait(false);

            var sut = new FieldDefinition(declaringType, node);

            sut.Attributes.Should().HaveCount(4);

            sut.Attributes.First().Name.Should().Be("First");
            sut.Attributes.Skip(1).First().Name.Should().Be("Second");
            sut.Attributes.Skip(2).First().Name.Should().Be("Third");
            sut.Attributes.Skip(3).First().Name.Should().Be("Fourth");
        }

        [Fact]
        public async Task DeclaringTypeReturnsParameterValue()
        {
            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(FieldDefinitionCode.GetSetField)
                .ConfigureAwait(false);

            var sut = new FieldDefinition(declaringType, node);

            sut.DeclaringType.Should().Be(declaringType);
        }

        [Fact]
        public async Task FullNameReturnsFieldNameCombinedWithParentFullName()
        {
            var parentFullName = Guid.NewGuid().ToString();

            var declaringType = Substitute.For<ITypeDefinition>();

            declaringType.FullName.Returns(parentFullName);

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(FieldDefinitionCode.GetSetField)
                .ConfigureAwait(false);

            var sut = new FieldDefinition(declaringType, node);

            sut.FullName.Should().Be(parentFullName + ".Value");
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("protected", true)]
        [InlineData("private protected", true)]
        [InlineData("protected internal", true)]
        [InlineData("public", true)]
        public async Task IsVisibleReturnsValueBasedOnScope(string scope, bool expected)
        {
            var code = FieldDefinitionCode.BuildFieldWithScope(scope);

            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new FieldDefinition(declaringType, node);

            sut.IsVisible.Should().Be(expected);
        }

        [Fact]
        public async Task LocationReturnsEmptyFilePathWhenNodeLacksSourceInformation()
        {
            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(FieldDefinitionCode.GetSetField)
                .ConfigureAwait(false);

            var sut = new FieldDefinition(declaringType, node);

            sut.Location.FilePath.Should().BeEmpty();
        }

        [Fact]
        public async Task LocationReturnsFileContentLocation()
        {
            var filePath = Guid.NewGuid().ToString();

            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode
                .FindNode<FieldDeclarationSyntax>(FieldDefinitionCode.GetSetField, filePath)
                .ConfigureAwait(false);

            var sut = new FieldDefinition(declaringType, node);

            sut.Location.LineIndex.Should().Be(5);
            sut.Location.CharacterIndex.Should().Be(8);
        }

        [Fact]
        public async Task LocationReturnsFilePathWhenNodeIncludesSourceInformation()
        {
            var filePath = Guid.NewGuid().ToString();

            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode
                .FindNode<FieldDeclarationSyntax>(FieldDefinitionCode.GetSetField, filePath)
                .ConfigureAwait(false);

            var sut = new FieldDefinition(declaringType, node);

            sut.Location.FilePath.Should().Be(filePath);
        }

        [Fact]
        public async Task NameReturnsFieldName()
        {
            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(FieldDefinitionCode.GetSetField)
                .ConfigureAwait(false);

            var sut = new FieldDefinition(declaringType, node);

            sut.Name.Should().Be("Value");
        }

        [Fact]
        public async Task ReturnTypeReturnsFieldType()
        {
            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(FieldDefinitionCode.GetSetField)
                .ConfigureAwait(false);

            var sut = new FieldDefinition(declaringType, node);

            sut.ReturnType.Should().Be("string");
        }

        [Fact]
        public async Task ReturnTypeReturnsGenericFieldType()
        {
            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(FieldDefinitionCode.GenericField)
                .ConfigureAwait(false);

            var sut = new FieldDefinition(declaringType, node);

            sut.ReturnType.Should().Be("T");
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification =
            "The constructor is the target of the test")]
        public async Task ThrowsExceptionWhenCreatedWithNullDeclaringType()
        {
            var node = await TestNode.FindNode<FieldDeclarationSyntax>(FieldDefinitionCode.GetSetField)
                .ConfigureAwait(false);

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new FieldDefinition(null!, node);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification =
            "The constructor is the target of the test")]
        public void ThrowsExceptionWhenCreatedWithNullNode()
        {
            var declaringType = Substitute.For<ITypeDefinition>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new FieldDefinition(declaringType, null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}