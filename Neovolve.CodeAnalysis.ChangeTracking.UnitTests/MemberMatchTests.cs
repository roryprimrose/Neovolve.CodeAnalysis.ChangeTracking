// ReSharper disable ObjectCreationAsStatement
namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
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
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing constructor guard clause")]
        public void ThrowsExceptionWhenCreatedWithNullNewNode()
        {
            var oldMember = Model.UsingModule<CompilerModule>().Create<MemberDefinition>();

            Action action = () => new MemberMatch(oldMember, null);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing constructor guard clause")]
        public void ThrowsExceptionWhenCreatedWithNullOldNode()
        {
            var newMember = Model.UsingModule<CompilerModule>().Create<MemberDefinition>();

            Action action = () => new MemberMatch(null, newMember);

            action.Should().Throw<ArgumentException>();
        }
    }
}