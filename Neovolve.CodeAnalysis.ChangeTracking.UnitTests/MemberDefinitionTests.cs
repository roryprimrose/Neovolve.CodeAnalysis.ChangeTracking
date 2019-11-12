namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class MemberDefinitionTests
    {
        [Theory]
        [InlineData(typeof(MemberDefinition))]
        [InlineData(typeof(PropertyDefinition))]
        [InlineData(typeof(AttributeDefinition))]
        public void ToStringReturnsMemberDescription(Type definitionType)
        {
            var sut = (MemberDefinition) Model.Create(definitionType);

            var actual = sut.ToString();

            actual.Should().StartWith(definitionType.Name);
            actual.Should().Contain(sut.Namespace);
            actual.Should().Contain(sut.OwningType);
            actual.Should().Contain(sut.Name);
        }

        [Theory]
        [InlineData(typeof(MemberDefinition))]
        [InlineData(typeof(PropertyDefinition))]
        [InlineData(typeof(AttributeDefinition))]
        public void ToStringReturnsMemberDescriptionWithoutNamespace(Type definitionType)
        {
            var sut = ((MemberDefinition) Model.Create(definitionType)).Set(x => x.Namespace = null);

            var actual = sut.ToString();

            actual.Should().StartWith(definitionType.Name);
            actual.Should().NotContain("..");
            actual.Should().Contain(sut.OwningType);
            actual.Should().Contain(sut.Name);
        }
    }
}