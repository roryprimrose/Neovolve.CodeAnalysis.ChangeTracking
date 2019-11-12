using System;
using System.Collections.Generic;
using FluentAssertions;
using ModelBuilder;
using Xunit;

namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    public class MatchResultsTests
    {
        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullMatches()
        {
            var oldMembersNotMatched = Model.Create<IEnumerable<MemberDefinition>>();
            var newMembersNotMatched = Model.Create<IEnumerable<MemberDefinition>>();

            Action action = () => new MatchResults(null, oldMembersNotMatched, newMembersNotMatched);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullOldMembersNotMatched()
        {
            var matches = Model.Create<IEnumerable<MemberMatch>>();
            var newMembersNotMatched = Model.Create<IEnumerable<MemberDefinition>>();

            Action action = () => new MatchResults(matches, null, newMembersNotMatched);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullNewMembersNotMatched()
        {
            var matches = Model.Create<IEnumerable<MemberMatch>>();
            var oldMembersNotMatched = Model.Create<IEnumerable<MemberDefinition>>();

            Action action = () => new MatchResults(matches, oldMembersNotMatched, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanCreateWithRequiredValues()
        {
            var matches = Model.Create<IList<MemberMatch>>();
            var oldMembersNotMatched = Model.Create<IList<MemberDefinition>>();
            var newMembersNotMatched = Model.Create<IList<MemberDefinition>>();

            var sut = new MatchResults(matches, oldMembersNotMatched, newMembersNotMatched);

            sut.Matches.Should().BeEquivalentTo(matches);
            sut.OldMembersNotMatched.Should().BeEquivalentTo(oldMembersNotMatched);
            sut.NewMembersNotMatched.Should().BeEquivalentTo(newMembersNotMatched);
        }
    }
}