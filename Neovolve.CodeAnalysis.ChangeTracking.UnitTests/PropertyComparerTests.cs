namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class PropertyComparerTests
    {
        [Theory]
        [InlineData(false, false, ChangeType.None)]
        [InlineData(true, true, ChangeType.None)]
        [InlineData(true, false, ChangeType.Breaking)]
        [InlineData(false, true, ChangeType.Feature)]
        public void CompareReturnsBaseResultWhenPropertyAccessorsHaveSameVisibility(
            bool oldValue,
            bool newValue,
            ChangeType expected)
        {
            var oldMember = Model.UsingModule<ConfigurationModule>()
                .Create<PropertyDefinition>()
                .Set(x => x.IsPublic = oldValue);
            var newMember = oldMember.JsonClone().Set(x => x.IsPublic = newValue);
            var match = new MemberMatch(oldMember, newMember);

            var sut = new PropertyComparer();

            var actual = sut.Compare(match);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CompareReturnsBreakingWhenFeatureAlsoIndicated()
        {
            var oldMember = Model.UsingModule<ConfigurationModule>()
                .Create<PropertyDefinition>()
                .Set(x => { x.CanWrite = false; });
            var newMember = oldMember.JsonClone()
                .Set(x =>
                {
                    x.ReturnType = Guid.NewGuid().ToString(); // Breaking
                    x.CanWrite = true; // Feature
                });
            var match = new MemberMatch(oldMember, newMember);

            var sut = new PropertyComparer();

            var actual = sut.Compare(match);

            actual.Should().Be(ChangeType.Breaking);
        }

        [Fact]
        public void CompareReturnsFeatureWhenBreakingChangeOnAccessorsAndPropertyNowVisible()
        {
            var oldMember = Model.UsingModule<ConfigurationModule>()
                .Create<PropertyDefinition>()
                .Set(x => { x.IsPublic = false; });
            var newMember = oldMember.JsonClone()
                .Set(x =>
                {
                    x.IsPublic = true;
                    x.CanWrite = false;
                });
            var match = new MemberMatch(oldMember, newMember);

            var sut = new PropertyComparer();

            var actual = sut.Compare(match);

            actual.Should().Be(ChangeType.Feature);
        }

        [Fact]
        public void CompareReturnsNoneWhenAccessorLessVisibleButPropertiesNotPublic()
        {
            var oldMember = Model.UsingModule<ConfigurationModule>()
                .Create<PropertyDefinition>()
                .Set(x =>
                {
                    x.IsPublic = false;
                    x.CanWrite = true;
                });
            var newMember = oldMember.JsonClone().Set(x => { x.CanWrite = false; });
            var match = new MemberMatch(oldMember, newMember);

            var sut = new PropertyComparer();

            var actual = sut.Compare(match);

            actual.Should().Be(ChangeType.None);
        }

        [Fact]
        public void CompareReturnsNoneWhenNodesMatch()
        {
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var newMember = oldMember.JsonClone();
            var match = new MemberMatch(oldMember, newMember);

            var sut = new PropertyComparer();

            var actual = sut.Compare(match);

            actual.Should().Be(ChangeType.None);
        }

        [Theory]
        [InlineData(false, false, ChangeType.None)]
        [InlineData(true, true, ChangeType.None)]
        [InlineData(false, true, ChangeType.Feature)]
        [InlineData(true, false, ChangeType.Breaking)]
        public void CompareReturnsResultOnChangesToGetAccessorVisibility(
            bool oldValue,
            bool newValue,
            ChangeType expected)
        {
            var oldMember = Model.UsingModule<ConfigurationModule>()
                .Create<PropertyDefinition>()
                .Set(x => x.CanRead = oldValue);
            var newMember = oldMember.JsonClone().Set(x => x.CanRead = newValue);
            var match = new MemberMatch(oldMember, newMember);

            var sut = new PropertyComparer();

            var actual = sut.Compare(match);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(false, false, ChangeType.None)]
        [InlineData(true, true, ChangeType.None)]
        [InlineData(false, true, ChangeType.Feature)]
        [InlineData(true, false, ChangeType.Breaking)]
        public void CompareReturnsResultOnChangesToSetAccessorVisibility(
            bool oldValue,
            bool newValue,
            ChangeType expected)
        {
            var oldMember = Model.UsingModule<ConfigurationModule>()
                .Create<PropertyDefinition>()
                .Set(x => x.CanWrite = oldValue);
            var newMember = oldMember.JsonClone().Set(x => x.CanWrite = newValue);
            var match = new MemberMatch(oldMember, newMember);

            var sut = new PropertyComparer();

            var actual = sut.Compare(match);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CompareThrowsExceptionWithNullMatch()
        {
            var sut = new PropertyComparer();

            Action action = () => sut.Compare(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(PropertyDefinition), true)]
        [InlineData(typeof(MemberDefinition), false)]
        [InlineData(typeof(AttributeDefinition), false)]
        public void IsSupportedReturnsTrueForExactTypeMatch(Type type, bool expected)
        {
            var definition = (MemberDefinition) Model.UsingModule<ConfigurationModule>().Create(type);

            var sut = new PropertyComparer();

            var actual = sut.IsSupported(definition);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullNode()
        {
            var sut = new PropertyComparer();

            Action action = () => sut.IsSupported(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}