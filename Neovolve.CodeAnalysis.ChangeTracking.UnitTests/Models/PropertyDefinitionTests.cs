namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class PropertyDefinitionTests
    {
        public static IEnumerable<object[]> AccessorScopeDataSet()
        {
            yield return new object[] {"", "", false};
            yield return new object[] {"", "private", false};
            yield return new object[] {"", "internal", false};
            yield return new object[] {"", "protected", false};
            yield return new object[] {"", "private protected", false};
            yield return new object[] {"", "protected internal", false};
            yield return new object[] {"", "public", false};
            yield return new object[] {"private", "", false};
            yield return new object[] {"private", "private", false};
            yield return new object[] {"private", "internal", false};
            yield return new object[] {"private", "protected", false};
            yield return new object[] {"private", "private protected", false};
            yield return new object[] {"private", "protected internal", false};
            yield return new object[] {"private", "public", false};
            yield return new object[] {"internal", "", false};
            yield return new object[] {"internal", "private", false};
            yield return new object[] {"internal", "internal", false};
            yield return new object[] {"internal", "protected", false};
            yield return new object[] {"internal", "private protected", false};
            yield return new object[] {"internal", "protected internal", false};
            yield return new object[] {"internal", "public", false};
            yield return new object[] {"protected", "", true};
            yield return new object[] {"protected", "private", false};
            yield return new object[] {"protected", "internal", false};
            yield return new object[] {"protected", "protected", true};
            yield return new object[] {"protected", "private protected", true};
            yield return new object[] {"protected", "protected internal", true};
            yield return new object[] {"protected", "public", true};
            yield return new object[] {"private protected", "", true};
            yield return new object[] {"private protected", "private", false};
            yield return new object[] {"private protected", "internal", false};
            yield return new object[] {"private protected", "protected", true};
            yield return new object[] {"private protected", "private protected", true};
            yield return new object[] {"private protected", "protected internal", true};
            yield return new object[] {"private protected", "public", true};
            yield return new object[] {"protected internal", "", true};
            yield return new object[] {"protected internal", "private", false};
            yield return new object[] {"protected internal", "internal", false};
            yield return new object[] {"protected internal", "protected", true};
            yield return new object[] {"protected internal", "private protected", true};
            yield return new object[] {"protected internal", "protected internal", true};
            yield return new object[] {"protected internal", "public", true};
            yield return new object[] {"public", "", true};
            yield return new object[] {"public", "private", false};
            yield return new object[] {"public", "internal", false};
            yield return new object[] {"public", "protected", true};
            yield return new object[] {"public", "private protected", true};
            yield return new object[] {"public", "protected internal", true};
            yield return new object[] {"public", "public", true};
        }

        [Fact]
        public async Task AttributesReturnsMultipleAttributesOnMultipleLists()
        {
            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode
                .FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode
                    .PropertyWithMultipleAttributesInMultipleLists)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.Attributes.Should().HaveCount(4);

            sut.Attributes.First().Name.Should().Be("First");
            sut.Attributes.Skip(1).First().Name.Should().Be("Second");
            sut.Attributes.Skip(2).First().Name.Should().Be("Third");
            sut.Attributes.Skip(3).First().Name.Should().Be("Fourth");
        }

        [Theory]
        [MemberData(nameof(AccessorScopeDataSet))]
        public async Task CanReadReturnsWhetherGetAccessorScopeAndPropertyScopeAreVisible(string propertyScope,
            string accessorScope, bool expected)
        {
            var code = PropertyDefinitionCode.BuildPropertyAndGetAccessorWithScope(propertyScope, accessorScope);

            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.CanRead.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(AccessorScopeDataSet))]
        public async Task CanReadReturnsWhetherSetAccessorScopeAndPropertyScopeAreVisible(string propertyScope,
            string accessorScope, bool expected)
        {
            var code = PropertyDefinitionCode.BuildPropertyAndSetAccessorWithScope(propertyScope, accessorScope);

            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.CanWrite.Should().Be(expected);
        }

        [Fact]
        public async Task DeclaringTypeReturnsParameterValue()
        {
            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GetSetProperty)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.DeclaringType.Should().Be(declaringType);
        }

        [Fact]
        public async Task FullNameReturnsPropertyNameCombinedWithParentFullName()
        {
            var parentFullName = Guid.NewGuid().ToString();

            var declaringType = Substitute.For<ITypeDefinition>();

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

            var declaringType = Substitute.For<ITypeDefinition>();

            declaringType.FullRawName.Returns(parentFullRawName);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GetSetProperty)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.FullRawName.Should().Be(parentFullRawName + ".Value");
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
            var code = PropertyDefinitionCode.BuildPropertyWithScope(scope);

            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.IsVisible.Should().Be(expected);
        }

        [Fact]
        public async Task LocationReturnsEmptyFilePathWhenNodeLacksSourceInformation()
        {
            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GetSetProperty)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.Location.FilePath.Should().BeEmpty();
        }

        [Fact]
        public async Task LocationReturnsFileContentLocation()
        {
            var filePath = Guid.NewGuid().ToString();

            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode
                .FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GetSetProperty, filePath)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.Location.LineIndex.Should().Be(5);
            sut.Location.CharacterIndex.Should().Be(8);
        }

        [Fact]
        public async Task LocationReturnsFilePathWhenNodeIncludesSourceInformation()
        {
            var filePath = Guid.NewGuid().ToString();

            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode
                .FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GetSetProperty, filePath)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.Location.FilePath.Should().Be(filePath);
        }

        [Fact]
        public async Task NameReturnsPropertyName()
        {
            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GetSetProperty)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.Name.Should().Be("Value");
        }

        [Fact]
        public async Task RawNameReturnsPropertyName()
        {
            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GetSetProperty)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.RawName.Should().Be("Value");
        }

        [Fact]
        public async Task ReturnTypeReturnsGenericPropertyType()
        {
            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GenericProperty)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.ReturnType.Should().Be("T");
        }

        [Fact]
        public async Task ReturnTypeReturnsPropertyType()
        {
            var declaringType = Substitute.For<ITypeDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GetSetProperty)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            sut.ReturnType.Should().Be("string");
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification =
            "The constructor is the target of the test")]
        public async Task ThrowsExceptionWhenCreatedWithNullDeclaringType()
        {
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GetSetProperty)
                .ConfigureAwait(false);

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PropertyDefinition(null!, node);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification =
            "The constructor is the target of the test")]
        public void ThrowsExceptionWhenCreatedWithNullNode()
        {
            var declaringType = Substitute.For<ITypeDefinition>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PropertyDefinition(declaringType, null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}