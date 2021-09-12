namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;
    using Xunit.Abstractions;

    public class InterfaceDefinitionTests
    {
        private readonly IChangeCalculator _calculator;
        private readonly ITestOutputHelper _output;

        public InterfaceDefinitionTests(ITestOutputHelper output)
        {
            _output = output;

            var logger = output.BuildLogger(LogLevel.Information);

            _calculator = ChangeCalculatorFactory.BuildCalculator(logger);
        }

        [Fact]
        public async Task CanCreateFromDeclarationNode()
        {
            var node = await TestNode.FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.EmptyInterface)
                .ConfigureAwait(false);

            var sut = new InterfaceDefinition(node);

            sut.Name.Should().Be("MyInterface");
            sut.Namespace.Should().Be("MyNamespace");
            sut.DeclaringType.Should().BeNull();
            sut.ChildClasses.Should().BeEmpty();
        }

        [Fact]
        public async Task CanParseInterfaceMethodAndDefaultMethod()
        {
            var node = await TestNode.FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.InterfaceWithMethodAndDefaultMethod)
                .ConfigureAwait(false);

            var sut = new InterfaceDefinition(node);

            sut.Methods.Should().HaveCount(2);
            
            var normalMethod = sut.Methods.First(x => x.Name == "GetValue(string, bool, int)");

            normalMethod.HasBody.Should().BeFalse();

            var defaultMethod = sut.Methods.First(x => x.Name == "DoSomething()");

            defaultMethod.HasBody.Should().BeTrue();
        }

        [Fact]
        public async Task GenericConstraintsReturnsDeclaredConstraints()
        {
            var node = await TestNode
                .FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.InterfaceWithGenericConstraints)
                .ConfigureAwait(false);

            var actual = new InterfaceDefinition(node);

            actual.GenericConstraints.Should().HaveCount(1);

            var constraintList = actual.GenericConstraints.First();

            constraintList.Name.Should().Be("T");
            constraintList.Constraints.First().Should().Be("Stream");
            constraintList.Constraints.Skip(1).First().Should().Be("new()");
        }

        [Fact]
        public async Task GenericConstraintsReturnsEmptyWhenNoConstraintsDeclared()
        {
            var node = await TestNode.FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.InterfaceWithoutParent)
                .ConfigureAwait(false);

            var actual = new InterfaceDefinition(node);

            actual.GenericConstraints.Should().BeEmpty();
        }

        [Fact]
        public async Task GenericConstraintsReturnsMultipleDeclaredConstraints()
        {
            var node = await TestNode
                .FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.InterfaceWithMultipleGenericConstraints)
                .ConfigureAwait(false);

            var actual = new InterfaceDefinition(node);

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
        [InlineData("", "", InterfaceModifiers.Partial)]
        [InlineData("new", "", InterfaceModifiers.NewPartial)]
        [InlineData("", "new", InterfaceModifiers.NewPartial)]
        [InlineData("new", "new", InterfaceModifiers.NewPartial)]
        public async Task MergePartialTypeMergesModifiers(string firstModifiers, string secondModifiers,
            InterfaceModifiers expected)
        {
            var firstCode =
                TypeDefinitionCode.EmptyInterface.Replace("interface", firstModifiers + " partial interface");
            var secondCode = TypeDefinitionCode.EmptyInterface
                .Replace("interface", secondModifiers + " partial interface");

            var firstNode = await TestNode.FindNode<InterfaceDeclarationSyntax>(firstCode)
                .ConfigureAwait(false);
            var secondNode = await TestNode.FindNode<InterfaceDeclarationSyntax>(secondCode)
                .ConfigureAwait(false);

            var firstDefinition = new InterfaceDefinition(firstNode);
            var secondDefinition = new InterfaceDefinition(secondNode);

            firstDefinition.MergePartialType(secondDefinition);

            firstDefinition.Modifiers.Should().Be(expected);
        }

        [Theory]
        [InlineData("", InterfaceModifiers.None)]
        [InlineData("new", InterfaceModifiers.New)]
        [InlineData("new partial", InterfaceModifiers.NewPartial)]
        [InlineData("partial", InterfaceModifiers.Partial)]
        public async Task ModifiersReturnsExpectedValue(string modifiers, InterfaceModifiers expected)
        {
            var code = TypeDefinitionCode.EmptyInterface.Replace("public interface MyInterface",
                "public " + modifiers + " interface MyInterface");

            var node = await TestNode.FindNode<InterfaceDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new InterfaceDefinition(node);

            sut.Modifiers.Should().Be(expected);
        }
    }
}