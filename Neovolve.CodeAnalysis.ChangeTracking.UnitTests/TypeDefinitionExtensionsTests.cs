namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class TypeDefinitionExtensionsTests
    {
        [Fact]
        public void GetNameWithoutGenericTypesReturnsNameWhenNotGenericType()
        {
            var sut = new TestClassDefinition();

            var actual = sut.GetNameWithoutGenericTypes();

            actual.Should().Be(sut.Name);
        }

        [Fact]
        public void GetNameWithoutGenericTypesReturnsNameWithoutGenericTypes()
        {
            var expected = Guid.NewGuid().ToString("N");
            var genericTypeParameters = new List<string> { "TKey","TValue"}.AsReadOnly();
            var sut = new TestClassDefinition
            {
                GenericTypeParameters = genericTypeParameters,
                Name = expected + "<TKey, TValue>"
            };

            var actual = sut.GetNameWithoutGenericTypes();

            actual.Should().Be(expected);
        }

        [Fact]
        public void GetNameWithoutGenericTypesThrowsExceptionWithNullDefinition()
        {
            Action action = () => TypeDefinitionExtensions.GetNameWithoutGenericTypes(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}