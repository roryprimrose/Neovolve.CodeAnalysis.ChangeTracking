﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class MemberDefinitionTests
    {
        [Theory]
        [InlineData(typeof(OldMemberDefinition))]
        [InlineData(typeof(OldPropertyDefinition))]
        [InlineData(typeof(OldAttributeDefinition))]
        public void ToStringReturnsMemberDescription(Type definitionType)
        {
            var sut = (OldMemberDefinition) Model.UsingModule<ConfigurationModule>().Create(definitionType);

            var actual = sut.ToString();

            actual.Should().StartWith(sut.MemberType.ToString());
            actual.Should().Contain(sut.Namespace);
            actual.Should().Contain(sut.OwningType);
            actual.Should().Contain(sut.Name);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ToStringReturnsMemberDescriptionWithOptionalMemberType(bool include)
        {
            var sut = Model.UsingModule<ConfigurationModule>().Create<OldMemberDefinition>();

            var actual = sut.ToString(include);

            if (include)
            {
                actual.Should().Contain(sut.MemberType.ToString());
            }
            else
            {
                actual.Should().NotContain(sut.MemberType.ToString());
            }
        }

        [Theory]
        [InlineData(typeof(OldMemberDefinition))]
        [InlineData(typeof(OldPropertyDefinition))]
        [InlineData(typeof(OldAttributeDefinition))]
        public void ToStringReturnsMemberDescriptionWithoutNamespace(Type definitionType)
        {
            var sut = ((OldMemberDefinition) Model.UsingModule<ConfigurationModule>().Create(definitionType)).Set(x =>
                x.Namespace = null);

            var actual = sut.ToString();

            actual.Should().StartWith(sut.MemberType.ToString());
            actual.Should().NotContain("..");
            actual.Should().Contain(sut.OwningType);
            actual.Should().Contain(sut.Name);
        }
    }
}