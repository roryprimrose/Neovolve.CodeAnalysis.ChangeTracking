namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models;
    using Xunit;

    public class IsVisibleExtensionsTests
    {
        [Fact]
        public void IsVisibleForAccessorThrowsExceptionWithNullDeclaration()
        {
            Action action = () => ((AccessorDeclarationSyntax) null!).IsVisible();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsVisibleForClassThrowsExceptionWithNullDeclaration()
        {
            Action action = () => ((ClassDeclarationSyntax) null!).IsVisible();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsVisibleForInterfaceThrowsExceptionWithNullDeclaration()
        {
            Action action = () => ((InterfaceDeclarationSyntax) null!).IsVisible();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsVisibleForMemberThrowsExceptionWithNullDeclaration()
        {
            Action action = () => ((MemberDeclarationSyntax) null!).IsVisible();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task IsVisibleReturnsTrueForInterfaceProperty()
        {
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(TypeDefinitionCode.InterfaceWithProperties)
                .ConfigureAwait(false);

            var actual = node.IsVisible();

            actual.Should().BeTrue();
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("public", true)]
        [InlineData("public readonly", true)]
        [InlineData("public virtual", true)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("internal virtual", false)]
        [InlineData("protected", true)]
        [InlineData("protected virtual", true)]
        public async Task IsVisibleReturnsValueBasedOnMemberModifier(
            string accessors,
            bool expected)
        {
            var code = TestNode.ClassProperty.Replace("public string MyProperty",
                accessors + " string MyProperty",
                StringComparison.Ordinal);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var actual = node.IsVisible();

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("public", true)]
        [InlineData("public readonly", true)]
        [InlineData("public virtual", true)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("internal virtual", false)]
        [InlineData("protected", true)]
        [InlineData("protected virtual", true)]
        public async Task IsVisibleReturnsValueBasedOnParentClassScopeForClassProperty(
            string accessors,
            bool expected)
        {
            var code = TestNode.ClassProperty.Replace("public class MyClass",
                accessors + " class MyClass",
                StringComparison.Ordinal);

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var actual = node.IsVisible();

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("public", true)]
        [InlineData("public readonly", true)]
        [InlineData("public virtual", true)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("internal virtual", false)]
        [InlineData("protected", true)]
        [InlineData("protected virtual", true)]
        public async Task IsVisibleReturnsValueBasedOnParentInterfaceScopeForInterfaceProperty(
            string accessors,
            bool expected)
        {
            var code = TestNode.InterfaceProperty.Replace("public interface MyInterface",
                accessors + " interface MyInterface",
                StringComparison.Ordinal);

            var node = await TestNode.FindNode<InterfaceDeclarationSyntax>(code).ConfigureAwait(false);

            var actual = node.IsVisible();

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("public", true)]
        [InlineData("public readonly", true)]
        [InlineData("public virtual", true)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("internal virtual", false)]
        [InlineData("protected", true)]
        [InlineData("protected virtual", true)]
        public async Task IsVisibleReturnsValueBasedOnPropertyAccessorModifier(
            string accessors,
            bool expected)
        {
            var code = TestNode.ClassProperty.Replace("get;", accessors + " get;", StringComparison.Ordinal);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var getAccessor =
                node.AccessorList?.Accessors.FirstOrDefault(x => x.Kind() == SyntaxKind.GetAccessorDeclaration);

            getAccessor.Should().NotBeNull();

            var actual = getAccessor!.IsVisible();

            actual.Should().Be(expected);
        }
    }
}