﻿// ReSharper disable ObjectCreationAsStatement
using System;
using System.Collections.Generic;
using FluentAssertions;
using ModelBuilder;
using Xunit;

namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System.Diagnostics.CodeAnalysis;

    public class MatchResultsTests
    {
        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing constructor guard clause")]
        public void ThrowsExceptionWhenCreatedWithNullMatches()
        {
            var oldMembersNotMatched = Model.UsingModule<CompilerModule>().Create<IEnumerable<MemberDefinition>>();
            var newMembersNotMatched = Model.UsingModule<CompilerModule>().Create<IEnumerable<MemberDefinition>>();

            Action action = () => new MatchResults(null, oldMembersNotMatched, newMembersNotMatched);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing constructor guard clause")]
        public void ThrowsExceptionWhenCreatedWithNullOldMembersNotMatched()
        {
            var matches = Model.UsingModule<CompilerModule>().Create<IEnumerable<MemberMatch>>();
            var newMembersNotMatched = Model.UsingModule<CompilerModule>().Create<IEnumerable<MemberDefinition>>();

            Action action = () => new MatchResults(matches, null, newMembersNotMatched);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing constructor guard clause")]
        public void ThrowsExceptionWhenCreatedWithNullNewMembersNotMatched()
        {
            var matches = Model.UsingModule<CompilerModule>().Create<IEnumerable<MemberMatch>>();
            var oldMembersNotMatched = Model.UsingModule<CompilerModule>().Create<IEnumerable<MemberDefinition>>();

            Action action = () => new MatchResults(matches, oldMembersNotMatched, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanCreateWithRequiredValues()
        {
            var matches = Model.UsingModule<CompilerModule>().Create<IList<MemberMatch>>();
            var oldMembersNotMatched = Model.UsingModule<CompilerModule>().Create<IList<MemberDefinition>>();
            var newMembersNotMatched = Model.UsingModule<CompilerModule>().Create<IList<MemberDefinition>>();

            var sut = new MatchResults(matches, oldMembersNotMatched, newMembersNotMatched);

            sut.Matches.Should().BeEquivalentTo(matches);
            sut.OldMembersNotMatched.Should().BeEquivalentTo(oldMembersNotMatched);
            sut.NewMembersNotMatched.Should().BeEquivalentTo(newMembersNotMatched);
        }
    }
}