namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class PropertyAccessorDefinitionTests
    {
        [Theory]
        [InlineData("", PropertyAccessorAccessModifier.None)]
        [InlineData("internal", PropertyAccessorAccessModifier.Internal)]
        [InlineData("protected", PropertyAccessorAccessModifier.Protected)]
        [InlineData("internal protected", PropertyAccessorAccessModifier.ProtectedInternal)]
        [InlineData("protected internal", PropertyAccessorAccessModifier.ProtectedInternal)]
        [InlineData("private", PropertyAccessorAccessModifier.Private)]
        public async Task AccessModifierReturnsExpectedValue(string modifiers, PropertyAccessorAccessModifier expected)
        {
            var parentName = Guid.NewGuid().ToString();

            var declaringProperty = Substitute.For<IPropertyDefinition>();

            declaringProperty.Name.Returns(parentName);

            var node = await TestNode
                .FindNode<AccessorDeclarationSyntax>(
                    PropertyDefinitionCode.ReadOnlyProperty.Replace("get;", modifiers + " get;"))
                .ConfigureAwait(false);

            var sut = new PropertyAccessorDefinition(declaringProperty, node);

            sut.AccessModifier.Should().Be(expected);
        }

        [Theory]
        [InlineData(PropertyDefinitionCode.ReadOnlyProperty, "_get")]
        [InlineData(PropertyDefinitionCode.WriteOnlyProperty, "_set")]
        public async Task FullNameReturnsPropertyNameCombinedWithParentFullName(string code, string expectedSuffix)
        {
            var parentFullName = Guid.NewGuid().ToString();

            var declaringProperty = Substitute.For<IPropertyDefinition>();

            declaringProperty.FullName.Returns(parentFullName);

            var node = await TestNode.FindNode<AccessorDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new PropertyAccessorDefinition(declaringProperty, node);

            sut.FullName.Should().Be(parentFullName + expectedSuffix);
        }

        [Theory]
        [InlineData(PropertyDefinitionCode.ReadOnlyProperty, "_get")]
        [InlineData(PropertyDefinitionCode.WriteOnlyProperty, "_set")]
        public async Task FullRawNameReturnsPropertyNameCombinedWithParentFullRawName(string code,
            string expectedSuffix)
        {
            var parentFullRawName = Guid.NewGuid().ToString();

            var declaringProperty = Substitute.For<IPropertyDefinition>();

            declaringProperty.FullRawName.Returns(parentFullRawName);

            var node = await TestNode.FindNode<AccessorDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new PropertyAccessorDefinition(declaringProperty, node);

            sut.FullRawName.Should().Be(parentFullRawName + expectedSuffix);
        }

        [Theory]
        [InlineData(true, "", true)]
        [InlineData(true, "internal", false)]
        [InlineData(true, "protected", true)]
        [InlineData(true, "internal protected", true)]
        [InlineData(true, "protected internal", true)]
        [InlineData(true, "private", false)]
        [InlineData(false, "", false)]
        [InlineData(false, "internal", false)]
        [InlineData(false, "protected", false)]
        [InlineData(false, "internal protected", false)]
        [InlineData(false, "protected internal", false)]
        [InlineData(false, "private", false)]
        public async Task IsVisibleReturnsValueBasedOnAccessModifierAndParentVisibility(bool parentIsVisible,
            string modifiers, bool expected)
        {
            var parentName = Guid.NewGuid().ToString();

            var declaringProperty = Substitute.For<IPropertyDefinition>();

            declaringProperty.Name.Returns(parentName);
            declaringProperty.IsVisible.Returns(parentIsVisible);

            var node = await TestNode
                .FindNode<AccessorDeclarationSyntax>(
                    PropertyDefinitionCode.ReadOnlyProperty.Replace("get;", modifiers + " get;"))
                .ConfigureAwait(false);

            var sut = new PropertyAccessorDefinition(declaringProperty, node);

            sut.IsVisible.Should().Be(expected);
        }

        [Theory]
        [InlineData(PropertyDefinitionCode.ReadOnlyProperty, "_get")]
        [InlineData(PropertyDefinitionCode.WriteOnlyProperty, "_set")]
        public async Task NameReturnsPropertyNameWithAccessorSuffix(string code, string expectedSuffix)
        {
            var parentName = Guid.NewGuid().ToString();

            var declaringProperty = Substitute.For<IPropertyDefinition>();

            declaringProperty.Name.Returns(parentName);

            var node = await TestNode.FindNode<AccessorDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new PropertyAccessorDefinition(declaringProperty, node);

            sut.Name.Should().Be(parentName + expectedSuffix);
        }

        [Theory]
        [InlineData(PropertyDefinitionCode.ReadOnlyProperty, "_get")]
        [InlineData(PropertyDefinitionCode.WriteOnlyProperty, "_set")]
        public async Task RawNameReturnsPropertyName(string code, string expectedSuffix)
        {
            var parentRawName = Guid.NewGuid().ToString();

            var declaringProperty = Substitute.For<IPropertyDefinition>();

            declaringProperty.RawName.Returns(parentRawName);

            var node = await TestNode.FindNode<AccessorDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new PropertyAccessorDefinition(declaringProperty, node);

            sut.RawName.Should().Be(parentRawName + expectedSuffix);
        }

        [Fact]
        [SuppressMessage(
            "Usage",
            "CA1806:Do not ignore method results",
            Justification = "The constructor is the target of the test")]
        public async Task ThrowsExceptionWhenCreatedWithNullDeclaringProperty()
        {
            var node = await TestNode.FindNode<AccessorDeclarationSyntax>(PropertyDefinitionCode.ReadOnlyProperty)
                .ConfigureAwait(false);

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PropertyAccessorDefinition(null!, node);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage(
            "Usage",
            "CA1806:Do not ignore method results",
            Justification = "The constructor is the target of the test")]
        public void ThrowsExceptionWhenCreatedWithNullNode()
        {
            var declaringProperty = Substitute.For<IPropertyDefinition>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PropertyAccessorDefinition(declaringProperty, null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}