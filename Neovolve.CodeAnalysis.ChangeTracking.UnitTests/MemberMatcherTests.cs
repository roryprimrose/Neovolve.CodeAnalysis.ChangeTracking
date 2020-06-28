namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class MemberMatcherTests
    {
        [Fact]
        public void GetMatchReturnsMatcherWhenNodesHaveSameIdentifiers()
        {
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<MemberDefinition>();
            var newMember = oldMember.JsonClone();

            var sut = new MemberMatcher();

            var actual = sut.GetMatch(oldMember, newMember);

            actual.Should().NotBeNull();
            actual!.NewMember.Should().Be(newMember);
            actual.OldMember.Should().Be(oldMember);
        }

        [Fact]
        public void GetMatchReturnsMatcherWhenNodesHaveSameIdentifiersWithNullNamespace()
        {
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<MemberDefinition>().Set(x => x.Namespace = null);
            var newMember = oldMember.JsonClone();

            var sut = new MemberMatcher();

            var actual = sut.GetMatch(oldMember, newMember);

            actual.Should().NotBeNull();
            actual!.NewMember.Should().Be(newMember);
            actual.OldMember.Should().Be(oldMember);
        }

        [Theory]
        [InlineData("Some", "Other")]
        [InlineData("Some", "some")]
        public void GetMatchReturnsNullWhereNameIsDifferent(string oldValue, string newValue)
        {
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<MemberDefinition>().Set(x => x.Name = oldValue);
            var newMember = oldMember.JsonClone().Set(x => x.Name = newValue);

            var sut = new MemberMatcher();

            var actual = sut.GetMatch(oldMember, newMember);

            actual.Should().BeNull();
        }

        [Theory]
        [InlineData("Some", "Other")]
        [InlineData("Some", "some")]
        public void GetMatchReturnsNullWhereNamespaceIsDifferent(string oldValue, string newValue)
        {
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<MemberDefinition>().Set(x => x.Namespace = oldValue);
            var newMember = oldMember.JsonClone().Set(x => x.Namespace = newValue);

            var sut = new MemberMatcher();

            var actual = sut.GetMatch(oldMember, newMember);

            actual.Should().BeNull();
        }

        [Theory]
        [InlineData("Some", "Other")]
        [InlineData("Some", "some")]
        public void GetMatchReturnsNullWhereOwningTypeIsDifferent(string oldValue, string newValue)
        {
            var oldMember = Model.UsingModule<ConfigurationModule>()
                .Create<MemberDefinition>()
                .Set(x => x.OwningType = oldValue);
            var newMember = oldMember.JsonClone().Set(x => x.OwningType = newValue);

            var sut = new MemberMatcher();

            var actual = sut.GetMatch(oldMember, newMember);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetMatchThrowsExceptionWithNullNewNode()
        {
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<MemberDefinition>();

            var sut = new MemberMatcher();

            Action action = () => sut.GetMatch(oldMember, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetMatchThrowsExceptionWithNullOldNode()
        {
            var newMember = Model.UsingModule<ConfigurationModule>().Create<MemberDefinition>();

            var sut = new MemberMatcher();

            Action action = () => sut.GetMatch(null!, newMember);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(MemberDefinition), true)]
        [InlineData(typeof(PropertyDefinition), true)]
        [InlineData(typeof(OldAttributeDefinition), false)]
        public void IsSupportedReturnsTrueForExactTypeMatch(Type type, bool expected)
        {
            var definition = (MemberDefinition) Model.UsingModule<ConfigurationModule>().Create(type);

            var sut = new MemberMatcher();

            var actual = sut.IsSupported(definition);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullNode()
        {
            var sut = new MemberMatcher();

            Action action = () => sut.IsSupported(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}