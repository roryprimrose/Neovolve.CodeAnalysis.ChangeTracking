namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;
    using Xunit.Abstractions;

    public class MemberComparerTests
    {
        private readonly ITestOutputHelper _output;

        public MemberComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareReturnsFeatureWhenReturnTypeChangedWithPropertyChangedToPublic()
        {
            var oldMember = Model.UsingModule<ConfigurationModule>()
                .Create<MemberDefinition>()
                .Set(x =>
                {
                    x.IsPublic = false;
                    x.ReturnType = "string";
                });
            var newMember = oldMember.JsonClone()
                .Set(x =>
                {
                    x.IsPublic = true; // Feature
                    x.ReturnType = "DateTimeOffset"; // Breaking
                });
            var match = new MemberMatch(oldMember, newMember);

            var sut = new MemberComparer();

            var actual = sut.Compare(match);

            _output.WriteLine(actual.Message);

            actual.ChangeType.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public void CompareReturnsNoneWhenNodesMatch()
        {
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<MemberDefinition>();
            var newMember = oldMember.JsonClone();
            var match = new MemberMatch(oldMember, newMember);

            var sut = new MemberComparer();

            var actual = sut.Compare(match);

            _output.WriteLine(actual.Message);

            actual.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Fact]
        public void CompareReturnsNoneWhenReturnTypeChangedWithPropertyNotPublic()
        {
            var oldMember = Model.UsingModule<ConfigurationModule>()
                .Create<MemberDefinition>()
                .Set(x =>
                {
                    x.IsPublic = false;
                    x.ReturnType = "string";
                });
            var newMember = oldMember.JsonClone().Set(x => { x.ReturnType = "DateTimeOffset"; });
            var match = new MemberMatch(oldMember, newMember);

            var sut = new MemberComparer();

            var actual = sut.Compare(match);

            _output.WriteLine(actual.Message);

            actual.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Theory]
        [InlineData(false, false, SemVerChangeType.None)]
        [InlineData(true, true, SemVerChangeType.None)]
        [InlineData(true, false, SemVerChangeType.Breaking)]
        [InlineData(false, true, SemVerChangeType.Feature)]
        public void CompareReturnsResultBasedOnIsPublic(bool oldValue, bool newValue, SemVerChangeType expected)
        {
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<MemberDefinition>()
                .Set(x => x.IsPublic = oldValue);
            var newMember = oldMember.JsonClone().Set(x => x.IsPublic = newValue);
            var match = new MemberMatch(oldMember, newMember);

            var sut = new MemberComparer();

            var actual = sut.Compare(match);

            _output.WriteLine(actual.Message);

            actual.ChangeType.Should().Be(expected);
        }

        [Theory]
        [InlineData("string", "string", SemVerChangeType.None)]
        [InlineData("string", "DateTimeOffset", SemVerChangeType.Breaking)]
        public void CompareReturnsResultBasedOnReturnType(string oldValue, string newValue, SemVerChangeType expected)
        {
            var oldMember = Model.UsingModule<ConfigurationModule>()
                .Create<MemberDefinition>()
                .Set(x => x.ReturnType = oldValue);
            var newMember = oldMember.JsonClone().Set(x => x.ReturnType = newValue);
            var match = new MemberMatch(oldMember, newMember);

            var sut = new MemberComparer();

            var actual = sut.Compare(match);

            _output.WriteLine(actual.Message);

            actual.ChangeType.Should().Be(expected);
        }

        [Theory]
        [InlineData(null, "NewValue")]
        [InlineData("", "NewValue")]
        [InlineData(" ", "NewValue")]
        [InlineData("OldValue", null)]
        [InlineData("OldValue", "")]
        [InlineData("OldValue", " ")]
        [InlineData("OldValue", "NewValue")]
        public void CompareThrowsExceptionWhenNameDoesNotMatch(string oldValue, string newValue)
        {
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<MemberDefinition>()
                .Set(x => x.Name = oldValue);
            var newMember = oldMember.JsonClone().Set(x => x.Name = newValue);
            var match = new MemberMatch(oldMember, newMember);

            var sut = new MemberComparer();

            Action action = () => sut.Compare(match);

            action.Should().Throw<InvalidOperationException>();
        }

        [Theory]
        [InlineData(null, "NewValue")]
        [InlineData("", "NewValue")]
        [InlineData(" ", "NewValue")]
        [InlineData("OldValue", null)]
        [InlineData("OldValue", "")]
        [InlineData("OldValue", " ")]
        [InlineData("OldValue", "NewValue")]
        public void CompareThrowsExceptionWhenNamespaceDoesNotMatch(string oldValue, string newValue)
        {
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<MemberDefinition>()
                .Set(x => x.Namespace = oldValue);
            var newMember = oldMember.JsonClone().Set(x => x.Namespace = newValue);
            var match = new MemberMatch(oldMember, newMember);

            var sut = new MemberComparer();

            Action action = () => sut.Compare(match);

            action.Should().Throw<InvalidOperationException>();
        }

        [Theory]
        [InlineData(null, "NewValue")]
        [InlineData("", "NewValue")]
        [InlineData(" ", "NewValue")]
        [InlineData("OldValue", null)]
        [InlineData("OldValue", "")]
        [InlineData("OldValue", " ")]
        [InlineData("OldValue", "NewValue")]
        public void CompareThrowsExceptionWhenOwningTypeDoesNotMatch(string oldValue, string newValue)
        {
            var oldMember = Model.UsingModule<ConfigurationModule>()
                .Create<MemberDefinition>()
                .Set(x => x.OwningType = oldValue);
            var newMember = oldMember.JsonClone().Set(x => x.OwningType = newValue);
            var match = new MemberMatch(oldMember, newMember);

            var sut = new MemberComparer();

            Action action = () => sut.Compare(match);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void CompareThrowsExceptionWithNullMatch()
        {
            var sut = new MemberComparer();

            Action action = () => sut.Compare(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(MemberDefinition), true)]
        [InlineData(typeof(PropertyDefinition), false)]
        [InlineData(typeof(AttributeDefinition), false)]
        public void IsSupportedReturnsTrueForExactTypeMatch(Type type, bool expected)
        {
            var definition = (MemberDefinition) Model.UsingModule<ConfigurationModule>().Create(type);

            var sut = new MemberComparer();

            var actual = sut.IsSupported(definition);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullNode()
        {
            var sut = new MemberComparer();

            Action action = () => sut.IsSupported(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}