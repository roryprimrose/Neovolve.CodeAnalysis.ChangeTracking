namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;

    public class TypeDefinitionExtensionsTests
    {
        [Fact]
        public void IsMatchReturnsFalseWhenGenericTypeParameterCountDoesNotMatch()
        {
            var oldType = new TestClassDefinition();
            var newType = oldType.JsonClone();

            newType.GenericTypeParameters = new[] {"test"};

            var actual = oldType.IsMatch(newType);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsFalseWhenNamespaceDoesNotMatch()
        {
            var oldType = new TestClassDefinition();
            var newType = oldType.JsonClone().Set(x => x.Namespace = Guid.NewGuid().ToString());

            var actual = oldType.IsMatch(newType);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsFalseWhenOnlyNewTypeHasParent()
        {
            var parent = new TestClassDefinition();
            var oldType = new TestClassDefinition();
            var newType = oldType.JsonClone().Set(x => x.DeclaringType = parent);

            var actual = oldType.IsMatch(newType);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsFalseWhenOnlyOldTypeHasParent()
        {
            var parent = new TestClassDefinition();
            var oldType = new TestClassDefinition();
            var newType = oldType.JsonClone();

            oldType.DeclaringType = parent;

            var actual = oldType.IsMatch(newType);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsFalseWhenParentTypesDoNotMatch()
        {
            var oldParent = new TestClassDefinition();
            var newParent = new TestClassDefinition();
            var oldType = new TestClassDefinition();
            var newType = oldType.JsonClone();

            oldType.DeclaringType = oldParent;
            newType.DeclaringType = newParent;

            var actual = oldType.IsMatch(newType);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenParentTypesMatch()
        {
            var oldParent = new TestClassDefinition();
            var newParent = oldParent.JsonClone();
            var oldType = new TestClassDefinition();
            var newType = oldType.JsonClone();

            oldType.DeclaringType = oldParent;
            newType.DeclaringType = newParent;

            var actual = oldType.IsMatch(newType);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenTypesMatch()
        {
            var oldType = new TestClassDefinition();
            var newType = oldType.JsonClone();

            var actual = oldType.IsMatch(newType);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullNewType()
        {
            var oldType = Substitute.For<ITypeDefinition>();

            Action action = () => oldType.IsMatch(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullOldType()
        {
            var newType = Substitute.For<ITypeDefinition>();

            Action action = () => TypeDefinitionExtensions.IsMatch(null!, newType);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}