namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class IsVisibleExtensionsTests
    {
        [Fact]
        public void IsVisibleForAccessorThrowsExceptionWithNullDeclaration()
        {
            var declaringProperty = Substitute.For<IPropertyDefinition>();

            Action action = () => IsVisibleExtensions.IsVisible(null!, declaringProperty);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("", true, false)]
        [InlineData("public", true, true)]
        [InlineData("private", true, false)]
        [InlineData("internal", true, false)]
        [InlineData("protected", true, true)]
        [InlineData("protected internal", true, true)]
        [InlineData("protected private", true, true)]
        [InlineData("", false, false)]
        [InlineData("public", false, false)]
        [InlineData("private", false, false)]
        [InlineData("internal", false, false)]
        [InlineData("protected", false, false)]
        [InlineData("protected internal", false, false)]
        [InlineData("protected private", false, false)]
        public async Task IsVisibleForMemberReturnsValueBasedOnModifier(
            string accessors,
            bool typeIsVisible,
            bool expected)
        {
            var code = TestNode.ClassProperty.Replace("public string MyProperty",
                accessors + " string MyProperty",
                StringComparison.Ordinal);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var declaringType = Substitute.For<IClassDefinition>();

            declaringType.IsVisible.Returns(typeIsVisible);

            var actual = node.IsVisible(declaringType);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsVisibleForMemberThrowsExceptionWithNullDeclaration()
        {
            var declaringType = Substitute.For<ITypeDefinition>();

            Action action = () => ((MemberDeclarationSyntax) null!).IsVisible(declaringType);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsVisibleForTypeThrowsExceptionWithNullDeclaration()
        {
            Action action = () => ((TypeDeclarationSyntax) null!).IsVisible(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task IsVisibleReturnsTrueForInterfaceProperty()
        {
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(TypeDefinitionCode.InterfaceWithProperties)
                .ConfigureAwait(false);

            var declaringType = Substitute.For<IInterfaceDefinition>();

            declaringType.IsVisible.Returns(true);

            var actual = node.IsVisible(declaringType);

            actual.Should().BeTrue();
        }
    }
}