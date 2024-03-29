﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
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

        [Fact]
        public void DetermineAccessModifierReturnsDefaultWhenNoModifierSpecified()
        {
            var defaultValue = AccessModifiers.Internal;
            var list = new SyntaxTokenList();

            var actual = list.DetermineAccessModifier(defaultValue);

            actual.Should().Be(defaultValue);
        }

        [Theory]
        [InlineData("", AccessModifiers.Internal)]
        [InlineData("private", AccessModifiers.Private)]
        [InlineData("internal", AccessModifiers.Internal)]
        [InlineData("protected", AccessModifiers.Protected)]
        [InlineData("private protected", AccessModifiers.ProtectedPrivate)]
        [InlineData("protected private", AccessModifiers.ProtectedPrivate)]
        [InlineData("protected internal", AccessModifiers.ProtectedInternal)]
        [InlineData("internal protected", AccessModifiers.ProtectedInternal)]
        [InlineData("public", AccessModifiers.Public)]
        public async Task DetermineAccessModifierReturnsValueBasedOnAccessModifiers(
            string accessModifiers,
            AccessModifiers expected)
        {
            var defaultValue = AccessModifiers.Internal;
            var code = TypeDefinitionCode.BuildClassWithScope(accessModifiers);

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var list = node.Modifiers;

            var actual = list.DetermineAccessModifier(defaultValue);

            actual.Should().Be(expected);
        }

        [Fact]
        public void DetermineEnumAccessModifierReturnsDefaultWhenNoModifierSpecified()
        {
            var defaultValue = EnumAccessModifiers.Internal;
            var list = new SyntaxTokenList();

            var actual = list.DetermineAccessModifier(defaultValue);

            actual.Should().Be(defaultValue);
        }

        [Theory]
        [InlineData("", EnumAccessModifiers.Internal)]
        [InlineData("private", EnumAccessModifiers.Private)]
        [InlineData("internal", EnumAccessModifiers.Internal)]
        [InlineData("protected", EnumAccessModifiers.Protected)]
        [InlineData("public", EnumAccessModifiers.Public)]
        public async Task DetermineEnumAccessModifierReturnsValueBasedOnEnumAccessModifiers(
            string accessModifiers,
            EnumAccessModifiers expected)
        {
            var defaultValue = EnumAccessModifiers.Internal;
            var code = TypeDefinitionCode.BuildClassWithScope(accessModifiers);

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var list = node.Modifiers;

            var actual = list.DetermineAccessModifier(defaultValue);

            actual.Should().Be(expected);
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