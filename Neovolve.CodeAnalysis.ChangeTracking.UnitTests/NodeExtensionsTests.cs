namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NSubstitute;
    using Xunit;

    public class NodeExtensionsTests
    {
        [Fact]
        public async Task DetermineAttributesReturnsEmptyWhenNoAttributesDeclared()
        {
            var declaringItem = Substitute.For<IMemberDefinition>();

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var actual = node.DetermineAttributes(declaringItem);

            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task DetermineAttributesReturnsMultipleAttributes()
        {
            var declaringItem = Substitute.For<IMemberDefinition>();

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithMultipleAttributes)
                .ConfigureAwait(false);

            var actual = node.DetermineAttributes(declaringItem);

            actual.Should().HaveCount(2);

            actual.First().Name.Should().Be("First");
            actual.Skip(1).First().Name.Should().Be("Second");
        }

        [Fact]
        public async Task DetermineAttributesReturnsMultipleAttributesOnMultipleLists()
        {
            var declaringItem = Substitute.For<IMemberDefinition>();

            var node = await TestNode
                .FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithMultipleAttributesInMultipleLists)
                .ConfigureAwait(false);

            var actual = node.DetermineAttributes(declaringItem);

            actual.Should().HaveCount(4);

            actual.First().Name.Should().Be("First");
            actual.Skip(1).First().Name.Should().Be("Second");
            actual.Skip(2).First().Name.Should().Be("Third");
            actual.Skip(3).First().Name.Should().Be("Fourth");
        }

        [Fact]
        public async Task DetermineAttributesReturnsMultipleAttributesOnSingleList()
        {
            var declaringItem = Substitute.For<IMemberDefinition>();

            var node = await TestNode
                .FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithMultipleAttributesInSingleList)
                .ConfigureAwait(false);

            var actual = node.DetermineAttributes(declaringItem);

            actual.Should().HaveCount(2);

            actual.First().Name.Should().Be("First");
            actual.Skip(1).First().Name.Should().Be("Second");
        }

        [Fact]
        public async Task DetermineAttributesReturnsSingleAttribute()
        {
            var declaringItem = Substitute.For<IMemberDefinition>();

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithSingleAttribute)
                .ConfigureAwait(false);

            var actual = node.DetermineAttributes(declaringItem);

            actual.Should().HaveCount(1);

            actual.First().Name.Should().Be("MyAttribute");
            actual.First().DeclaredOn.Should().Be(declaringItem);
        }

        [Fact]
        public async Task DetermineAttributesThrowsExceptionWithNullDeclaringItem()
        {
            var node = await TestNode
                .FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithMultipleAttributesInMultipleLists)
                .ConfigureAwait(false);

            Action action = () => node.DetermineAttributes(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DetermineAttributesThrowsExceptionWithNullNode()
        {
            var declaringItem = Substitute.For<IMemberDefinition>();

            Action action = () => NodeExtensions.DetermineAttributes(null!, declaringItem);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task DetermineLocationReturnsEmptyFilePathWhenNoFileDefined()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var actual = node.DetermineLocation();

            actual.FilePath.Should().BeEmpty();
        }

        [Fact]
        public async Task DetermineLocationReturnsFilePathWhenFileDefined()
        {
            var filePath = Guid.NewGuid().ToString();

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent, filePath)
                .ConfigureAwait(false);

            var actual = node.DetermineLocation();

            actual.FilePath.Should().Be(filePath);
        }

        [Fact]
        public async Task DetermineLocationReturnsPositionValues()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var actual = node.DetermineLocation();

            actual.LineIndex.Should().Be(3);
            actual.CharacterIndex.Should().Be(4);
        }

        [Fact]
        public void DetermineLocationThrowsExceptionWithNullNode()
        {
            Action action = () => NodeExtensions.DetermineLocation(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task DetermineNamespaceReturnsEmptyWhenNoNamespaceFound()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutNamespace)
                .ConfigureAwait(false);

            var actual = node.DetermineNamespace();

            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task DetermineNamespaceReturnsOwningComplexNamespace()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithComplexNamespace)
                .ConfigureAwait(false);

            var actual = node.DetermineNamespace();

            actual.Should().Be("MyNamespace.OtherNamespace.FinalNamespace");
        }

        [Fact]
        public async Task DetermineNamespaceReturnsOwningNamespace()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var actual = node.DetermineNamespace();

            actual.Should().Be("MyNamespace");
        }

        [Fact]
        public void DetermineNamespaceThrowsExceptionWithNullNode()
        {
            Action action = () => NodeExtensions.DetermineNamespace(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}