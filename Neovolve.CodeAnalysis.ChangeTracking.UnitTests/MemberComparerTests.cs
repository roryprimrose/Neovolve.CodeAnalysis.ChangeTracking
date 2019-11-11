namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class MemberComparerTests
    {
        [Fact]
        public void CompareReturnsFeatureWhenReturnTypeChangedWithPropertyChangedToPublic()
        {
            var oldMember = Model.UsingModule<CompilerModule>()
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

            var sut = new MemberComparer();

            var actual = sut.Compare(oldMember, newMember);

            actual.Should().Be(ChangeType.Feature);
        }

        [Fact]
        public void CompareReturnsNoneWhenNodesMatch()
        {
            var oldMember = Model.UsingModule<CompilerModule>().Create<MemberDefinition>();
            var newMember = oldMember.JsonClone();

            var sut = new MemberComparer();

            var actual = sut.Compare(oldMember, newMember);

            actual.Should().Be(ChangeType.None);
        }

        [Fact]
        public void CompareReturnsNoneWhenReturnTypeChangedWithPropertyNotPublic()
        {
            var oldMember = Model.UsingModule<CompilerModule>()
                .Create<MemberDefinition>()
                .Set(x =>
                {
                    x.IsPublic = false;
                    x.ReturnType = "string";
                });
            var newMember = oldMember.JsonClone().Set(x => { x.ReturnType = "DateTimeOffset"; });

            var sut = new MemberComparer();

            var actual = sut.Compare(oldMember, newMember);

            actual.Should().Be(ChangeType.None);
        }

        [Theory]
        [InlineData(false, false, ChangeType.None)]
        [InlineData(true, true, ChangeType.None)]
        [InlineData(true, false, ChangeType.Breaking)]
        [InlineData(false, true, ChangeType.Feature)]
        public void CompareReturnsResultBasedOnIsPublic(bool oldValue, bool newValue, ChangeType expected)
        {
            var oldMember = Model.UsingModule<CompilerModule>().Create<MemberDefinition>().Set(x => x.IsPublic = oldValue);
            var newMember = oldMember.JsonClone().Set(x => x.IsPublic = newValue);

            var sut = new MemberComparer();

            var actual = sut.Compare(oldMember, newMember);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("string", "string", ChangeType.None)]
        [InlineData("string", "DateTimeOffset", ChangeType.Breaking)]
        public void CompareReturnsResultBasedOnReturnType(string oldValue, string newValue, ChangeType expected)
        {
            var oldMember = Model.UsingModule<CompilerModule>()
                .Create<MemberDefinition>()
                .Set(x => x.ReturnType = oldValue);
            var newMember = oldMember.JsonClone().Set(x => x.ReturnType = newValue);

            var sut = new MemberComparer();

            var actual = sut.Compare(oldMember, newMember);

            actual.Should().Be(expected);
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
            var oldMember = Model.UsingModule<CompilerModule>().Create<MemberDefinition>().Set(x => x.Name = oldValue);
            var newMember = oldMember.JsonClone().Set(x => x.Name = newValue);

            var sut = new MemberComparer();

            Action action = () => sut.Compare(oldMember, newMember);

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
            var oldMember = Model.UsingModule<CompilerModule>().Create<MemberDefinition>().Set(x => x.Namespace = oldValue);
            var newMember = oldMember.JsonClone().Set(x => x.Namespace = newValue);

            var sut = new MemberComparer();

            Action action = () => sut.Compare(oldMember, newMember);

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
            var oldMember = Model.UsingModule<CompilerModule>()
                .Create<MemberDefinition>()
                .Set(x => x.OwningType = oldValue);
            var newMember = oldMember.JsonClone().Set(x => x.OwningType = newValue);

            var sut = new MemberComparer();

            Action action = () => sut.Compare(oldMember, newMember);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void CompareThrowsExceptionWithNullNewNode()
        {
            var oldMember = Model.UsingModule<CompilerModule>().Create<MemberDefinition>();

            var sut = new MemberComparer();

            Action action = () => sut.Compare(oldMember, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CompareThrowsExceptionWithNullOldNode()
        {
            var newMember = Model.UsingModule<CompilerModule>().Create<MemberDefinition>();

            var sut = new MemberComparer();

            Action action = () => sut.Compare(null, newMember);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(MemberDefinition), true)]
        [InlineData(typeof(PropertyDefinition), false)]
        [InlineData(typeof(AttributeDefinition), false)]
        public void IsSupportedReturnsTrueForExactTypeMatch(Type type, bool expected)
        {
            var definition = (MemberDefinition) Model.Create(type);

            var sut = new MemberComparer();

            var actual = sut.IsSupported(definition);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullNode()
        {
            var sut = new MemberComparer();

            Action action = () => sut.IsSupported(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}