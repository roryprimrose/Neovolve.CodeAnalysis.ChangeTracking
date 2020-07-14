namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models;
    using Xunit;

    public class ConstraintListDefinitionTests
    {
        [Fact]
        public async Task ConstrainsReturnsDefinedGenericTypeConstraints()
        {
            var node = await TestNode
                .FindNode<TypeParameterConstraintClauseSyntax>(TypeDefinitionCode.ClassWithGenericConstraints)
                .ConfigureAwait(false);

            var sut = new ConstraintListDefinition(node);

            sut.Name.Should().Be("T");
            sut.Constraints.Should().HaveCount(2);
            sut.Constraints.First().Should().Be("Stream");
            sut.Constraints.Skip(1).First().Should().Be("new()");
        }

        [Fact]
        public async Task LocationReturnsEmptyFilePathWhenNodeLacksSourceInformation()
        {
            var node = await TestNode
                .FindNode<TypeParameterConstraintClauseSyntax>(TypeDefinitionCode.ClassWithGenericConstraints)
                .ConfigureAwait(false);

            var sut = new ConstraintListDefinition(node);

            sut.Location.FilePath.Should().BeEmpty();
        }

        [Fact]
        public async Task LocationReturnsFileContentLocation()
        {
            var filePath = Guid.NewGuid().ToString();

            var node = await TestNode
                .FindNode<TypeParameterConstraintClauseSyntax>(TypeDefinitionCode.ClassWithGenericConstraints, filePath)
                .ConfigureAwait(false);

            var sut = new ConstraintListDefinition(node);

            sut.Location.LineIndex.Should().Be(3);
            sut.Location.CharacterIndex.Should().Be(45);
        }

        [Fact]
        public async Task LocationReturnsFilePathWhenNodeIncludesSourceInformation()
        {
            var filePath = Guid.NewGuid().ToString();

            var node = await TestNode
                .FindNode<TypeParameterConstraintClauseSyntax>(TypeDefinitionCode.ClassWithGenericConstraints, filePath)
                .ConfigureAwait(false);

            var sut = new ConstraintListDefinition(node);

            sut.Location.FilePath.Should().Be(filePath);
        }

        [Fact]
        public async Task NameReturnsConstraintName()
        {
            var node = await TestNode
                .FindNode<TypeParameterConstraintClauseSyntax>(TypeDefinitionCode.ClassWithGenericConstraints)
                .ConfigureAwait(false);

            var sut = new ConstraintListDefinition(node);

            sut.Name.Should().Be("T");
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification =
            "The constructor is the target of the test")]
        public void ThrowsExceptionWhenCreatedWithNullNode()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ConstraintListDefinition(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}