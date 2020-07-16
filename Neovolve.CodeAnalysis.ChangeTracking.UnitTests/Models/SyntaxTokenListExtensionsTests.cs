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
        [InlineData("public static", SyntaxKind.StaticKeyword, true)]
        public async Task HasModifierReturnsWhetherListContainsModifier(
            string accessModifiers,
            SyntaxKind kind,
            bool expected)
        {
            var code = TypeDefinitionCode.BuildClassWithScope(accessModifiers);

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var list = node.Modifiers;

            var actual = list.HasModifier(kind);

            actual.Should().Be(expected);
        }
    }
}