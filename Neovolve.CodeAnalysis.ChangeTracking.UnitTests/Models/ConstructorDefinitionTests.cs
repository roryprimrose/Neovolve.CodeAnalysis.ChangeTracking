namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class ConstructorDefinitionTests
    {
        [Theory]
        [InlineData("", AccessModifiers.Private)]
        [InlineData("public", AccessModifiers.Public)]
        [InlineData("internal", AccessModifiers.Internal)]
        [InlineData("private", AccessModifiers.Private)]
        [InlineData("protected", AccessModifiers.Protected)]
        [InlineData("protected private", AccessModifiers.ProtectedPrivate)]
        [InlineData("protected internal", AccessModifiers.ProtectedInternal)]
        public async Task AccessModifiersReturnsExpectedValue(string modifiers, AccessModifiers expected)
        {
            var declaringType = new TestClassDefinition();
            var code = DefaultConstructor.Replace("public MyClass", modifiers + " MyClass");

            var node = await TestNode.FindNode<ConstructorDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new ConstructorDefinition(declaringType, node);

            sut.AccessModifiers.Should().Be(expected);
        }

        [Fact]
        public async Task AttributesReturnsDefinedValues()
        {
            var declaringType = new TestClassDefinition();

            var node = await TestNode.FindNode<ConstructorDeclarationSyntax>(ConstructorWithAttributes)
                .ConfigureAwait(false);

            var sut = new ConstructorDefinition(declaringType, node);

            sut.Attributes.Should().HaveCount(2);
            sut.Attributes.First().Name.Should().Be("First");
            sut.Attributes.Skip(1).First().Name.Should().Be("Second");
        }

        [Fact]
        public async Task AttributesReturnsEmptyOnWhenNoneDefined()
        {
            var declaringType = new TestClassDefinition();

            var node = await TestNode.FindNode<ConstructorDeclarationSyntax>(DefaultConstructor)
                .ConfigureAwait(false);

            var sut = new ConstructorDefinition(declaringType, node);

            sut.Attributes.Should().BeEmpty();
        }

        [Fact]
        public async Task DefaultConstructorReturnsEmptyParameters()
        {
            var declaringType = new TestClassDefinition();

            var node = await TestNode.FindNode<ConstructorDeclarationSyntax>(DefaultConstructor)
                .ConfigureAwait(false);

            var sut = new ConstructorDefinition(declaringType, node);

            sut.Parameters.Should().BeEmpty();
        }

        [Theory]
        [InlineData("", ConstructorModifiers.None)]
        [InlineData("static", ConstructorModifiers.Static)]
        public async Task ModifiersReturnsExpectedValue(string modifiers, ConstructorModifiers expected)
        {
            var declaringType = new TestClassDefinition();
            var code = DefaultConstructor.Replace("public MyClass", "public " + modifiers + " MyClass");

            var node = await TestNode.FindNode<ConstructorDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new ConstructorDefinition(declaringType, node);

            sut.Modifiers.Should().Be(expected);
        }

        [Theory]
        [InlineData("", "ctor")]
        [InlineData("static", "cctor")]
        public async Task NamesReturnsExpectedValue(string modifiers, string expected)
        {
            var declaringType = new TestClassDefinition();
            var code = DefaultConstructor.Replace("public MyClass", "public " + modifiers + " MyClass");

            var node = await TestNode.FindNode<ConstructorDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new ConstructorDefinition(declaringType, node);

            sut.Name.Should().Be(expected);
            sut.RawName.Should().Be(expected);
            sut.FullName.Should().Be(declaringType.FullName + "." + expected);
            sut.FullRawName.Should().Be(declaringType.FullRawName + "." + expected);
        }

        [Fact]
        public async Task ParametersReturnsDefinedValues()
        {
            var declaringType = new TestClassDefinition();

            var node = await TestNode.FindNode<ConstructorDeclarationSyntax>(ParameterConstructor)
                .ConfigureAwait(false);

            var sut = new ConstructorDefinition(declaringType, node);

            sut.Parameters.Should().HaveCount(3);
            sut.Parameters.First().Type.Should().Be("string");
            sut.Parameters.First().Name.Should().Be("first");
            sut.Parameters.Skip(1).First().Type.Should().Be("bool");
            sut.Parameters.Skip(1).First().Name.Should().Be("second");
            sut.Parameters.Skip(2).First().Type.Should().Be("int");
            sut.Parameters.Skip(2).First().Name.Should().Be("third");
        }

        [Fact]
        public async Task ParametersReturnsEmptyOnDefaultConstructor()
        {
            var declaringType = new TestClassDefinition();

            var node = await TestNode.FindNode<ConstructorDeclarationSyntax>(DefaultConstructor)
                .ConfigureAwait(false);

            var sut = new ConstructorDefinition(declaringType, node);

            sut.Parameters.Should().BeEmpty();
        }

        [Fact]
        public async Task ReturnTypeReturnsEmpty()
        {
            var declaringType = new TestClassDefinition();

            var node = await TestNode.FindNode<ConstructorDeclarationSyntax>(DefaultConstructor)
                .ConfigureAwait(false);

            var sut = new ConstructorDefinition(declaringType, node);

            sut.ReturnType.Should().BeEmpty();
        }

        private static string ConstructorWithAttributes => @"
namespace MyNamespace 
{
    public class MyClass
    {
        [First]
        [Second(""here"", true, 123)]
        public MyClass()
        {
        }
    }  
}
";

        private static string DefaultConstructor => @"
namespace MyNamespace 
{
    public class MyClass
    {
        public MyClass()
        {
        }
    }  
}
";

        private static string ParameterConstructor => @"
namespace MyNamespace 
{
    public class MyClass
    {
        public MyClass(string first, bool second, int third)
        {
        }
    }  
}
";
    }
}