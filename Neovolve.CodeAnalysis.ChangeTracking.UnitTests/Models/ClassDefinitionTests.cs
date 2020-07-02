namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
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

        [Theory]
        [InlineData("", false)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("protected", false)]
        [InlineData("private protected", false)]
        [InlineData("protected internal", false)]
        [InlineData("protected private", false)]
        [InlineData("internal protected", false)]
        [InlineData("public", false)]
        [InlineData("private abstract", true)]
        [InlineData("internal abstract", true)]
        [InlineData("protected abstract", true)]
        [InlineData("private protected abstract", true)]
        [InlineData("protected internal abstract", true)]
        [InlineData("protected private abstract", true)]
        [InlineData("internal protected abstract", true)]
        [InlineData("public abstract", true)]
        [InlineData("private sealed", false)]
        [InlineData("internal sealed", false)]
        [InlineData("protected sealed", false)]
        [InlineData("private protected sealed", false)]
        [InlineData("protected internal sealed", false)]
        [InlineData("protected private sealed", false)]
        [InlineData("internal protected sealed", false)]
        [InlineData("public sealed", false)]
        [InlineData("private static", false)]
        [InlineData("internal static", false)]
        [InlineData("protected static", false)]
        [InlineData("private protected static", false)]
        [InlineData("protected internal static", false)]
        [InlineData("protected private static", false)]
        [InlineData("internal protected static", false)]
        [InlineData("public static", false)]
        [InlineData("private partial", false)]
        [InlineData("internal partial", false)]
        [InlineData("protected partial", false)]
        [InlineData("private protected partial", false)]
        [InlineData("protected internal partial", false)]
        [InlineData("protected private partial", false)]
        [InlineData("internal protected partial", false)]
        [InlineData("public partial", false)]
        public async Task IsAbstractReturnsWhetherClassDeclaresSealedModifier(string modifiers, bool expected)
        {
            var code = EmptyClass.Replace("public class MyClass", modifiers + " class MyClass");

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.IsAbstract.Should().Be(expected);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("protected", false)]
        [InlineData("private protected", false)]
        [InlineData("protected internal", false)]
        [InlineData("protected private", false)]
        [InlineData("internal protected", false)]
        [InlineData("public", false)]
        [InlineData("private abstract", false)]
        [InlineData("internal abstract", false)]
        [InlineData("protected abstract", false)]
        [InlineData("private protected abstract", false)]
        [InlineData("protected internal abstract", false)]
        [InlineData("protected private abstract", false)]
        [InlineData("internal protected abstract", false)]
        [InlineData("public abstract", false)]
        [InlineData("private sealed", false)]
        [InlineData("internal sealed", false)]
        [InlineData("protected sealed", false)]
        [InlineData("private protected sealed", false)]
        [InlineData("protected internal sealed", false)]
        [InlineData("protected private sealed", false)]
        [InlineData("internal protected sealed", false)]
        [InlineData("public sealed", false)]
        [InlineData("private static", false)]
        [InlineData("internal static", false)]
        [InlineData("protected static", false)]
        [InlineData("private protected static", false)]
        [InlineData("protected internal static", false)]
        [InlineData("protected private static", false)]
        [InlineData("internal protected static", false)]
        [InlineData("public static", false)]
        [InlineData("private partial", true)]
        [InlineData("internal partial", true)]
        [InlineData("protected partial", true)]
        [InlineData("private protected partial", true)]
        [InlineData("protected internal partial", true)]
        [InlineData("protected private partial", true)]
        [InlineData("internal protected partial", true)]
        [InlineData("public partial", true)]
        public async Task IsPartialReturnsWhetherClassDeclaresSealedModifier(string modifiers, bool expected)
        {
            var code = EmptyClass.Replace("public class MyClass", modifiers + " class MyClass");

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.IsPartial.Should().Be(expected);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("protected", false)]
        [InlineData("private protected", false)]
        [InlineData("protected internal", false)]
        [InlineData("protected private", false)]
        [InlineData("internal protected", false)]
        [InlineData("public", false)]
        [InlineData("private abstract", false)]
        [InlineData("internal abstract", false)]
        [InlineData("protected abstract", false)]
        [InlineData("private protected abstract", false)]
        [InlineData("protected internal abstract", false)]
        [InlineData("protected private abstract", false)]
        [InlineData("internal protected abstract", false)]
        [InlineData("public abstract", false)]
        [InlineData("private sealed", true)]
        [InlineData("internal sealed", true)]
        [InlineData("protected sealed", true)]
        [InlineData("private protected sealed", true)]
        [InlineData("protected internal sealed", true)]
        [InlineData("protected private sealed", true)]
        [InlineData("internal protected sealed", true)]
        [InlineData("public sealed", true)]
        [InlineData("private static", false)]
        [InlineData("internal static", false)]
        [InlineData("protected static", false)]
        [InlineData("private protected static", false)]
        [InlineData("protected internal static", false)]
        [InlineData("protected private static", false)]
        [InlineData("internal protected static", false)]
        [InlineData("public static", false)]
        [InlineData("private partial", false)]
        [InlineData("internal partial", false)]
        [InlineData("protected partial", false)]
        [InlineData("private protected partial", false)]
        [InlineData("protected internal partial", false)]
        [InlineData("protected private partial", false)]
        [InlineData("internal protected partial", false)]
        [InlineData("public partial", false)]
        public async Task IsSealedReturnsWhetherClassDeclaresSealedModifier(string modifiers, bool expected)
        {
            var code = EmptyClass.Replace("public class MyClass", modifiers + " class MyClass");

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.IsSealed.Should().Be(expected);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("protected", false)]
        [InlineData("private protected", false)]
        [InlineData("protected internal", false)]
        [InlineData("protected private", false)]
        [InlineData("internal protected", false)]
        [InlineData("public", false)]
        [InlineData("private abstract", false)]
        [InlineData("internal abstract", false)]
        [InlineData("protected abstract", false)]
        [InlineData("private protected abstract", false)]
        [InlineData("protected internal abstract", false)]
        [InlineData("protected private abstract", false)]
        [InlineData("internal protected abstract", false)]
        [InlineData("public abstract", false)]
        [InlineData("private sealed", false)]
        [InlineData("internal sealed", false)]
        [InlineData("protected sealed", false)]
        [InlineData("private protected sealed", false)]
        [InlineData("protected internal sealed", false)]
        [InlineData("protected private sealed", false)]
        [InlineData("internal protected sealed", false)]
        [InlineData("public sealed", false)]
        [InlineData("private static", true)]
        [InlineData("internal static", true)]
        [InlineData("protected static", true)]
        [InlineData("private protected static", true)]
        [InlineData("protected internal static", true)]
        [InlineData("protected private static", true)]
        [InlineData("internal protected static", true)]
        [InlineData("public static", true)]
        [InlineData("private partial", false)]
        [InlineData("internal partial", false)]
        [InlineData("protected partial", false)]
        [InlineData("private protected partial", false)]
        [InlineData("protected internal partial", false)]
        [InlineData("protected private partial", false)]
        [InlineData("internal protected partial", false)]
        [InlineData("public partial", false)]
        public async Task IsStaticReturnsWhetherClassDeclaresSealedModifier(string modifiers, bool expected)
        {
            var code = EmptyClass.Replace("public class MyClass", modifiers + " class MyClass");

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.IsStatic.Should().Be(expected);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("private", "private")]
        [InlineData("internal", "internal")]
        [InlineData("protected", "protected")]
        [InlineData("private protected", "private protected")]
        [InlineData("protected internal", "protected internal")]
        [InlineData("protected private", "protected private")]
        [InlineData("internal protected", "internal protected")]
        [InlineData("public", "public")]
        [InlineData("private abstract", "private")]
        [InlineData("internal abstract", "internal")]
        [InlineData("protected abstract", "protected")]
        [InlineData("private protected abstract", "private protected")]
        [InlineData("protected internal abstract", "protected internal")]
        [InlineData("protected private abstract", "protected private")]
        [InlineData("internal protected abstract", "internal protected")]
        [InlineData("public abstract", "public")]
        [InlineData("private sealed", "private")]
        [InlineData("internal sealed", "internal")]
        [InlineData("protected sealed", "protected")]
        [InlineData("private protected sealed", "private protected")]
        [InlineData("protected internal sealed", "protected internal")]
        [InlineData("protected private sealed", "protected private")]
        [InlineData("internal protected sealed", "internal protected")]
        [InlineData("public sealed", "public")]
        [InlineData("private static", "private")]
        [InlineData("internal static", "internal")]
        [InlineData("protected static", "protected")]
        [InlineData("private protected static", "private protected")]
        [InlineData("protected internal static", "protected internal")]
        [InlineData("protected private static", "protected private")]
        [InlineData("internal protected static", "internal protected")]
        [InlineData("public static", "public")]
        [InlineData("private partial", "private")]
        [InlineData("internal partial", "internal")]
        [InlineData("protected partial", "protected")]
        [InlineData("private protected partial", "private protected")]
        [InlineData("protected internal partial", "protected internal")]
        [InlineData("protected private partial", "protected private")]
        [InlineData("internal protected partial", "internal protected")]
        [InlineData("public partial", "public")]
        public async Task ScopeReturnsScopeModifiers(string modifiers, string expected)
        {
            var code = EmptyClass.Replace("public class MyClass", modifiers + " class MyClass");

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Scope.Should().Be(expected);
        }
    }
}