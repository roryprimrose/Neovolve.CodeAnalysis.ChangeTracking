namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class MemberMatchTests
    {
        [Fact]
        public void CanCreateWithTwoNodes()
        {
            var oldMember = Model.UsingModule<CompilerModule>().Create<MemberDefinition>();
            var newMember = Model.UsingModule<CompilerModule>().Create<MemberDefinition>();

            var sut = new MemberMatch(oldMember, newMember);

            sut.OldMember.Should().Be(oldMember);
            sut.NewMember.Should().Be(newMember);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullNewNode()
        {
            var oldMember = Model.UsingModule<CompilerModule>().Create<MemberDefinition>();

            Action action = () => new MemberMatch(oldMember, null);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullOldNode()
        {
            var newMember = Model.UsingModule<CompilerModule>().Create<MemberDefinition>();

            Action action = () => new MemberMatch(null, newMember);

            action.Should().Throw<ArgumentException>();
        }
    }
}