namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class ElementDefinitionTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("private")]
        [InlineData("internal")]
        [InlineData("protected")]
        [InlineData("private protected")]
        [InlineData("protected internal")]
        [InlineData("public")]
        public async Task AccessModifiersReturnsValueBasedOnAccessModifiers(string accessModifiers)
        {
            var code = TypeDefinitionCode.BuildClassWithScope(accessModifiers);

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new Wrapper(node);

            sut.AccessModifiers.Should().Be(accessModifiers);
        }

        [Fact]
        public async Task AttributesReturnsEmptyWhenNoAttributesDeclared()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var sut = new Wrapper(node);

            sut.Attributes.Should().BeEmpty();
        }

        [Fact]
        public async Task AttributesReturnsMultipleAttributes()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithMultipleAttributes)
                .ConfigureAwait(false);

            var sut = new Wrapper(node);

            sut.Attributes.Should().HaveCount(2);

            sut.Attributes.First().Name.Should().Be("First");
            sut.Attributes.Skip(1).First().Name.Should().Be("Second");
        }

        [Fact]
        public async Task AttributesReturnsMultipleAttributesOnMultipleLists()
        {
            var node = await TestNode
                .FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithMultipleAttributesInMultipleLists)
                .ConfigureAwait(false);

            var sut = new Wrapper(node);

            sut.Attributes.Should().HaveCount(4);

            sut.Attributes.First().Name.Should().Be("First");
            sut.Attributes.Skip(1).First().Name.Should().Be("Second");
            sut.Attributes.Skip(2).First().Name.Should().Be("Third");
            sut.Attributes.Skip(3).First().Name.Should().Be("Fourth");
        }

        [Fact]
        public async Task AttributesReturnsMultipleAttributesOnSingleList()
        {
            var node = await TestNode
                .FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithMultipleAttributesInSingleList)
                .ConfigureAwait(false);

            var sut = new Wrapper(node);

            sut.Attributes.Should().HaveCount(2);

            sut.Attributes.First().Name.Should().Be("First");
            sut.Attributes.Skip(1).First().Name.Should().Be("Second");
        }

        [Fact]
        public async Task AttributesReturnsSingleAttribute()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithSingleAttribute)
                .ConfigureAwait(false);

            var sut = new Wrapper(node);

            sut.Attributes.Should().HaveCount(1);

            sut.Attributes.First().Name.Should().Be("MyAttribute");
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("protected", true)]
        [InlineData("private protected", true)]
        [InlineData("protected internal", true)]
        [InlineData("public", true)]
        public async Task IsVisibleReturnsValueBasedOnAccessModifiers(string accessModifiers, bool expected)
        {
            var code = TypeDefinitionCode.BuildClassWithScope(accessModifiers);

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new Wrapper(node);

            sut.IsVisible.Should().Be(expected);
        }

        [Fact]
        public async Task LocationReturnsEmptyFilePathWhenNodeLacksSourceInformation()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var sut = new Wrapper(node);

            sut.Location.FilePath.Should().BeEmpty();
        }

        [Fact]
        public async Task LocationReturnsFileContentLocation()
        {
            var filePath = Guid.NewGuid().ToString();

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent, filePath)
                .ConfigureAwait(false);

            var sut = new Wrapper(node);

            sut.Location.LineIndex.Should().Be(3);
            sut.Location.CharacterIndex.Should().Be(4);
        }

        [Fact]
        public async Task LocationReturnsFilePathWhenNodeIncludesSourceInformation()
        {
            var filePath = Guid.NewGuid().ToString();

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent, filePath)
                .ConfigureAwait(false);

            var sut = new Wrapper(node);

            sut.Location.FilePath.Should().Be(filePath);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abstract")]
        [InlineData("new")]
        [InlineData("override")]
        [InlineData("sealed")]
        [InlineData("static")]
        [InlineData("virtual")]
        [InlineData("new abstract")]
        [InlineData("new static")]
        [InlineData("new virtual")]
        [InlineData("sealed override")]
        public async Task ModifiersReturnsValueBasedOnModifiers(string modifiers)
        {
            var code = PropertyDefinitionCode.BuildPropertyWithModifiers(modifiers);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new Wrapper(node);

            sut.Modifiers.Should().Be(modifiers);
        }

        [Fact]
        [SuppressMessage(
            "Usage",
            "CA1806:Do not ignore method results",
            Justification = "The constructor is the target of the test")]
        public void ThrowsExceptionWhenCreatedWithNullNode()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Wrapper(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : ElementDefinition
        {
            public Wrapper(MemberDeclarationSyntax node) : base(node)
            {
                Description = Guid.NewGuid().ToString();
                Name = Guid.NewGuid().ToString();
                RawName = Guid.NewGuid().ToString();
                FullName = Guid.NewGuid().ToString();
                FullRawName = Guid.NewGuid().ToString();
            }

            public override string Description { get; }
            public override string FullName { get; }
            public override string FullRawName { get; }
            public override string Name { get; }
            public override string RawName { get; }
        }
    }
}