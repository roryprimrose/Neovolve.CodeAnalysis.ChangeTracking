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
        public void GetFullNameWithoutGenericTypesReturnsFullNameWhenNotGenericType()
        {
            var sut = new TestClassDefinition();

            var actual = sut.GetFullNameWithoutGenericTypes();

            actual.Should().Be(sut.FullName);
        }

        [Fact]
        public void GetFullNameWithoutGenericTypesReturnsFullNameWithoutGenericTypes()
        {
            var expected = Guid.NewGuid().ToString("N");
            var genericTypeParameters = new List<string> { "TKey","TValue"}.AsReadOnly();
            var sut = new TestClassDefinition
            {
                GenericTypeParameters = genericTypeParameters,
                FullName = expected + "<TKey, TValue>"
            };

            var actual = sut.GetFullNameWithoutGenericTypes();

            actual.Should().Be(expected);
        }

        [Fact]
        public void GetFullNameWithoutGenericTypesThrowsExceptionWithNullDefinition()
        {
            Action action = () => TypeDefinitionExtensions.GetFullNameWithoutGenericTypes(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}