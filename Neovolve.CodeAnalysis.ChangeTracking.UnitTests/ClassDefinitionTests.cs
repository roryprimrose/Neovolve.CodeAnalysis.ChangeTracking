namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    public class ClassDefinitionTests
    {
        private const string EmptyClass = @"
namespace MyNamespace 
{
    public class MyClass
    {
    }   
}
";

        [Fact]
        public async Task CanCreateFromDeclarationNode()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(EmptyClass)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Name.Should().Be("MyClass");
            sut.Namespace.Should().Be("MyNamespace");
            sut.ParentType.Should().BeNull();
            sut.ChildClasses.Should().BeEmpty();
        }

        [Fact]
        public async Task GenericConstraintsReturnsDeclaredConstraints()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithGenericConstraints)
                .ConfigureAwait(false);

            var actual = new ClassDefinition(node);

            actual.GenericConstraints.Should().HaveCount(1);
            
            var constraintList = actual.GenericConstraints.First();

            constraintList.Name.Should().Be("T");
            constraintList.Constraints.First().Should().Be("Stream");
            constraintList.Constraints.Skip(1).First().Should().Be("new()");
        }

        [Fact]
        public async Task GenericConstraintsReturnsMultipleDeclaredConstraints()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithMultipleGenericConstraints)
                .ConfigureAwait(false);

            var actual = new ClassDefinition(node);

            actual.GenericConstraints.Should().HaveCount(2);

            var firstConstraintList = actual.GenericConstraints.First();

            firstConstraintList.Name.Should().Be("TKey");
            firstConstraintList.Constraints.First().Should().Be("Stream");
            firstConstraintList.Constraints.Skip(1).First().Should().Be("new()");

            var secondConstraintList = actual.GenericConstraints.Skip(1).First();

            secondConstraintList.Name.Should().Be("TValue");
            secondConstraintList.Constraints.First().Should().Be("struct");
        }

        [Fact]
        public async Task GenericConstraintsReturnsEmptyWhenNoConstraintsDeclared()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var actual = new ClassDefinition(node);

            actual.GenericConstraints.Should().BeEmpty();
        }
    }
}