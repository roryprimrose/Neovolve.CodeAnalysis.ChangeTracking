namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class SyntaxTokenListExtensionsTests
    {
        private const string EmptyClass = @"
namespace MyNamespace 
{
    public class MyClass
    {
    }   
}
";

        [Theory]
        [InlineData("", AccessModifier.Internal)]
        [InlineData("private", AccessModifier.Private)]
        [InlineData("internal", AccessModifier.Internal)]
        [InlineData("protected", AccessModifier.Protected)]
        [InlineData("private protected", AccessModifier.ProtectedPrivate)]
        [InlineData("protected private", AccessModifier.ProtectedPrivate)]
        [InlineData("protected internal", AccessModifier.ProtectedInternal)]
        [InlineData("internal protected", AccessModifier.ProtectedInternal)]
        [InlineData("public", AccessModifier.Public)]
        public async Task AccessModifierReturnsValueBasedOnAccessModifiers(
            string accessModifiers,
            AccessModifier expected)
        {
            var defaultValue = AccessModifier.Internal;
            var code = TypeDefinitionCode.BuildClassWithScope(accessModifiers);

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var list = node.Modifiers;

            var actual = list.DetermineAccessModifier(defaultValue);

            actual.Should().Be(expected);
        }

        [Fact]
        public void DetermineAccessModifierReturnsDefaultWhenNoModifierSpecified()
        {
            var defaultValue = AccessModifier.Internal;
            var list = new SyntaxTokenList();

            var actual = list.DetermineAccessModifier(defaultValue);

            actual.Should().Be(defaultValue);
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

            var actual = node.Modifiers.HasModifier(kind);

            actual.Should().Be(expected);
        }
    }
}