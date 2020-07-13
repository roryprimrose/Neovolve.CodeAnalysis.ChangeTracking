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

    public class ElementDefinitionTests
    {
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

        [Fact]
        public async Task AttributesReturnsMultipleAttributesOnMultipleListsDeclaredOnProperty()
        {
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.PropertyWithMultipleAttributesInMultipleLists)
                .ConfigureAwait(false);

            var sut = new Wrapper(node);

            sut.Attributes.Should().HaveCount(4);

            sut.Attributes.First().Name.Should().Be("First");
            sut.Attributes.Skip(1).First().Name.Should().Be("Second");
            sut.Attributes.Skip(2).First().Name.Should().Be("Third");
            sut.Attributes.Skip(3).First().Name.Should().Be("Fourth");
        }

        [Fact]
        public async Task AttributesReturnsMultipleAttributesOnMultipleListsDeclaredOnPropertyAccessor()
        {
            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.PropertyAccessorWithMultipleAttributesInMultipleLists)
                .ConfigureAwait(false);

            var sut = new PropertyDefinition(declaringType, node);

            var attributes = sut.GetAccessor!.Attributes;
            attributes.Should().HaveCount(4);

            attributes.First().Name.Should().Be("First");
            attributes.Skip(1).First().Name.Should().Be("Second");
            attributes.Skip(2).First().Name.Should().Be("Third");
            attributes.Skip(3).First().Name.Should().Be("Fourth");
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
                IsVisible = true;
            }

            public override string Description { get; }
            public override string FullName { get; }
            public override string FullRawName { get; }
            public override bool IsVisible { get; }
            public override string Name { get; }
            public override string RawName { get; }
        }
    }
}