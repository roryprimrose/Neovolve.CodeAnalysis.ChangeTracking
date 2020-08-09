namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class StructDefinitionTests
    {
        private const string EmptyStruct = @"
namespace MyNamespace 
{
    public struct MyStruct
    {
    }   
}
";

        private const string StructWithFields = @"
namespace MyNamespace 
{
    public struct MyStruct
    {
        public string First;
        public DateTimeOffset Second;
    }   
}
";

        [Fact]
        public async Task CanCreateFromDeclarationNode()
        {
            var node = await TestNode.FindNode<StructDeclarationSyntax>(EmptyStruct)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.Name.Should().Be("MyStruct");
            sut.Namespace.Should().Be("MyNamespace");
            sut.DeclaringType.Should().BeNull();
            sut.ChildStructs.Should().BeEmpty();
        }

        [Fact]
        public async Task FieldsReturnsDeclaredFields()
        {
            var node = await TestNode.FindNode<StructDeclarationSyntax>(StructWithFields)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.Fields.Should().HaveCount(2);

            var first = sut.Fields.First();

            first.Name.Should().Be("First");
            first.IsVisible.Should().BeTrue();
            first.ReturnType.Should().Be("string");

            var second = sut.Fields.Skip(1).First();

            second.Name.Should().Be("Second");
            second.IsVisible.Should().BeTrue();
            second.ReturnType.Should().Be("DateTimeOffset");
        }

        [Fact]
        public async Task GenericConstraintsReturnsDeclaredConstraints()
        {
            var node = await TestNode.FindNode<StructDeclarationSyntax>(TypeDefinitionCode.StructWithGenericConstraints)
                .ConfigureAwait(false);

            var actual = new StructDefinition(node);

            actual.GenericConstraints.Should().HaveCount(1);

            var constraintList = actual.GenericConstraints.First();

            constraintList.Name.Should().Be("T");
            constraintList.Constraints.First().Should().Be("Stream");
            constraintList.Constraints.Skip(1).First().Should().Be("new()");
        }

        [Fact]
        public async Task GenericConstraintsReturnsEmptyWhenNoConstraintsDeclared()
        {
            var node = await TestNode.FindNode<StructDeclarationSyntax>(TypeDefinitionCode.StructWithoutParent)
                .ConfigureAwait(false);

            var actual = new StructDefinition(node);

            actual.GenericConstraints.Should().BeEmpty();
        }

        [Fact]
        public async Task GenericConstraintsReturnsMultipleDeclaredConstraints()
        {
            var node = await TestNode
                .FindNode<StructDeclarationSyntax>(TypeDefinitionCode.StructWithMultipleGenericConstraints)
                .ConfigureAwait(false);

            var actual = new StructDefinition(node);

            actual.GenericConstraints.Should().HaveCount(2);

            var firstConstraintList = actual.GenericConstraints.First();

            firstConstraintList.Name.Should().Be("TKey");
            firstConstraintList.Constraints.First().Should().Be("Stream");
            firstConstraintList.Constraints.Skip(1).First().Should().Be("new()");

            var secondConstraintList = actual.GenericConstraints.Skip(1).First();

            secondConstraintList.Name.Should().Be("TValue");
            secondConstraintList.Constraints.First().Should().Be("struct");
        }

        [Theory]
        [InlineData("", StructModifiers.None)]
        [InlineData("readonly", StructModifiers.ReadOnly)]
        [InlineData("readonly partial", StructModifiers.ReadOnlyPartial)]
        [InlineData("partial", StructModifiers.Partial)]
        public async Task ModifiersReturnsExpectedValue(string modifiers, StructModifiers expected)
        {
            var code = EmptyStruct.Replace("public struct MyStruct", "public " + modifiers + " struct MyStruct");

            var node = await TestNode.FindNode<StructDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.Modifiers.Should().Be(expected);
        }
    }
}