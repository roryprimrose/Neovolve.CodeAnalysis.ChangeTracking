namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class NodeExtensionsTests
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
        [InlineData("private sealed", "private")]
        [InlineData("internal sealed", "internal")]
        [InlineData("protected sealed", "protected")]
        [InlineData("private protected sealed", "private protected")]
        [InlineData("protected internal sealed", "protected internal")]
        [InlineData("protected private sealed", "protected private")]
        [InlineData("internal protected sealed", "internal protected")]
        [InlineData("public sealed", "public")]
        [InlineData("private abstract", "private")]
        [InlineData("internal abstract", "internal")]
        [InlineData("protected abstract", "protected")]
        [InlineData("private protected abstract", "private protected")]
        [InlineData("protected internal abstract", "protected internal")]
        [InlineData("protected private abstract", "protected private")]
        [InlineData("internal protected abstract", "internal protected")]
        [InlineData("public abstract", "public")]
        public async Task DetermineScopeReturnsScopeModifiers(string modifiers, string expected)
        {
            var code = EmptyClass.Replace("public class MyClass", modifiers + " class MyClass");

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var actual = node.DetermineScope();

            actual.Should().Be(expected);
        }

        [Fact]
        public void DetermineScopeThrowsExceptionWithNullNode()
        {
            Action action = () => NodeExtensions.DetermineScope(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("", SyntaxKind.PublicKeyword, false)]
        [InlineData("public", SyntaxKind.PublicKeyword, true)]
        [InlineData("private", SyntaxKind.PrivateKeyword, true)]
        [InlineData("internal", SyntaxKind.InternalKeyword, true)]
        [InlineData("protected", SyntaxKind.ProtectedKeyword, true)]
        [InlineData("sealed", SyntaxKind.SealedKeyword, true)]
        [InlineData("static", SyntaxKind.StaticKeyword, true)]
        [InlineData("new", SyntaxKind.NewKeyword, true)]
        [InlineData("abstract", SyntaxKind.AbstractKeyword, true)]
        [InlineData("partial", SyntaxKind.PartialKeyword, true)]
        [InlineData("internal protected abstract", SyntaxKind.PublicKeyword, false)]
        [InlineData("internal protected abstract", SyntaxKind.AbstractKeyword, true)]
        [InlineData("internal protected abstract", SyntaxKind.ProtectedKeyword, true)]
        [InlineData("internal protected abstract", SyntaxKind.InternalKeyword, true)]
        public async Task HasModifierReturnsWhetherModifierExists(string modifiers, SyntaxKind kind, bool expected)
        {
            var code = EmptyClass.Replace("public class MyClass", modifiers + " class MyClass");

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var actual = node.HasModifier(kind);

            actual.Should().Be(expected);
        }

        [Fact]
        public void HasModifierThrowsExceptionWithNullNode()
        {
            Action action = () => NodeExtensions.HasModifier(null!, SyntaxKind.SealedKeyword);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}