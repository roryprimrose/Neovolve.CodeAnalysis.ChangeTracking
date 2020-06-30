﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    public class ClassDefinitionTests
    {
        private const string ClassWithFields = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public string First;
        public DateTimeOffset Second;
    }   
}
";

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
            sut.DeclaringType.Should().BeNull();
            sut.ChildClasses.Should().BeEmpty();
        }

        [Fact]
        public async Task FieldsReturnsDeclaredFields()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(ClassWithFields)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

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
        public async Task GenericConstraintsReturnsEmptyWhenNoConstraintsDeclared()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var actual = new ClassDefinition(node);

            actual.GenericConstraints.Should().BeEmpty();
        }

        [Fact]
        public async Task GenericConstraintsReturnsMultipleDeclaredConstraints()
        {
            var node = await TestNode
                .FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithMultipleGenericConstraints)
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
    }
}