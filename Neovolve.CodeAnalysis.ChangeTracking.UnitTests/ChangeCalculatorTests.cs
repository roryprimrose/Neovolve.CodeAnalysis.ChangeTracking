// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable ObjectCreationAsStatement

namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class ChangeCalculatorTests
    {
        private readonly ILogger _logger;

        public ChangeCalculatorTests(ITestOutputHelper output)
        {
            _logger = output.BuildLogger();
        }

        [Fact]
        public async Task CompareChangesReturnsBreakingWhenOldPublicMemberHasNoNewMemberMatch()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};
            var oldMemberNotMatched = Model.UsingModule<CompilerModule>().Create<MemberDefinition>();
            var oldMembersNotMatched = new List<MemberDefinition> {oldMemberNotMatched};
            var newMembersNotMatched = new List<MemberDefinition>();
            var matches = new List<MemberMatch>();
            var results = new MatchResults(matches, oldMembersNotMatched, newMembersNotMatched);
            var oldNodes = new List<SyntaxNode>
            {
                await TestNode.Parse(TestNode.StandardProperty).ConfigureAwait(false)
            };
            var newNodes = new List<SyntaxNode>();

            evaluator.CompareNodes(oldNodes, newNodes).Returns(results);

            var sut = new ChangeCalculator(evaluator, comparers, _logger);

            var actual = sut.CalculateChange(oldNodes, newNodes);

            actual.Should().Be(ChangeType.Breaking);
        }

        [Fact]
        public void CompareChangesReturnsBreakingWhenSubsequentMatchesFoundNonBreakingChanges()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};
            var oldMember = Model.UsingModule<CompilerModule>().Create<MemberDefinition>();
            var newMember = oldMember.JsonClone();
            var oldMembersNotMatched = new List<MemberDefinition>();
            var newMembersNotMatched = new List<MemberDefinition>();
            var matches = new List<MemberMatch>
            {
                new MemberMatch(oldMember, newMember),
                new MemberMatch(oldMember, newMember),
                new MemberMatch(oldMember, newMember)
            };

            var results = new MatchResults(matches, oldMembersNotMatched, newMembersNotMatched);
            var oldNodes = new List<SyntaxNode>();

            var newNodes = new List<SyntaxNode>();

            evaluator.CompareNodes(oldNodes, newNodes).Returns(results);
            comparer.IsSupported(Arg.Any<MemberDefinition>()).Returns(true);
            comparer.Compare(Arg.Any<MemberMatch>()).Returns(ChangeType.None, ChangeType.Feature, ChangeType.Breaking);

            var sut = new ChangeCalculator(evaluator, comparers, _logger);

            var actual = sut.CalculateChange(oldNodes, newNodes);

            actual.Should().Be(ChangeType.Breaking);
        }

        [Theory]
        [InlineData(ChangeType.None)]
        [InlineData(ChangeType.Feature)]
        [InlineData(ChangeType.Breaking)]
        public void CompareChangesReturnsComparerValueForMemberMatch(ChangeType changeType)
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};
            var oldMember = Model.UsingModule<CompilerModule>().Create<MemberDefinition>();
            var newMember = oldMember.JsonClone();
            var oldMembersNotMatched = new List<MemberDefinition>();
            var newMembersNotMatched = new List<MemberDefinition>();
            var matches = new List<MemberMatch>
            {
                new MemberMatch(oldMember, newMember)
            };

            var results = new MatchResults(matches, oldMembersNotMatched, newMembersNotMatched);
            var oldNodes = new List<SyntaxNode>();

            var newNodes = new List<SyntaxNode>();

            evaluator.CompareNodes(oldNodes, newNodes).Returns(results);
            comparer.IsSupported(oldMember).Returns(true);
            comparer.IsSupported(newMember).Returns(true);
            comparer.Compare(matches[0]).Returns(changeType);

            var sut = new ChangeCalculator(evaluator, comparers, _logger);

            var actual = sut.CalculateChange(oldNodes, newNodes);

            actual.Should().Be(changeType);
        }

        [Fact]
        public async Task CompareChangesReturnsFeatureWhenNewPublicMemberHasNoOldMemberMatchAndNoMatchesFound()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};
            var oldMembersNotMatched = new List<MemberDefinition>();
            var newMemberNotMatched = Model.UsingModule<CompilerModule>().Create<MemberDefinition>();
            var newMembersNotMatched = new List<MemberDefinition> {newMemberNotMatched};
            var matches = new List<MemberMatch>();
            var results = new MatchResults(matches, oldMembersNotMatched, newMembersNotMatched);
            var oldNodes = new List<SyntaxNode>
            {
                await TestNode.Parse(TestNode.StandardProperty).ConfigureAwait(false)
            };
            var newNodes = new List<SyntaxNode>();

            evaluator.CompareNodes(oldNodes, newNodes).Returns(results);

            var sut = new ChangeCalculator(evaluator, comparers, _logger);

            var actual = sut.CalculateChange(oldNodes, newNodes);

            actual.Should().Be(ChangeType.Feature);
        }

        [Fact]
        public void CompareChangesReturnsNoneWhenResultsAreEmpty()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};
            var oldMembersNotMatched = new List<MemberDefinition>();
            var newMembersNotMatched = new List<MemberDefinition>();
            var matches = new List<MemberMatch>();
            var results = new MatchResults(matches, oldMembersNotMatched, newMembersNotMatched);
            var oldNodes = new List<SyntaxNode>();
            var newNodes = new List<SyntaxNode>();

            evaluator.CompareNodes(oldNodes, newNodes).Returns(results);

            var sut = new ChangeCalculator(evaluator, comparers, _logger);

            var actual = sut.CalculateChange(oldNodes, newNodes);

            actual.Should().Be(ChangeType.None);
        }

        [Fact]
        public void CompareChangesReturnsNullWhenNoResultsFound()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};
            var oldNodes = new List<SyntaxNode>();
            var newNodes = new List<SyntaxNode>();

            var sut = new ChangeCalculator(evaluator, comparers, _logger);

            var actual = sut.CalculateChange(oldNodes, newNodes);

            actual.Should().Be(ChangeType.None);
        }

        [Fact]
        public void CompareChangesThrowsExceptionWhenCreatedWithNullNewNodes()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};
            var oldNodes = new List<SyntaxNode>();

            var sut = new ChangeCalculator(evaluator, comparers, _logger);

            Action action = () => sut.CalculateChange(oldNodes, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CompareChangesThrowsExceptionWhenCreatedWithNullOldNodes()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};
            var newNodes = new List<SyntaxNode>();

            var sut = new ChangeCalculator(evaluator, comparers, _logger);

            Action action = () => sut.CalculateChange(null, newNodes);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CompareChangesThrowsExceptionWhenNoComparerFoundForMember()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};
            var oldMember = Model.UsingModule<CompilerModule>().Create<MemberDefinition>();
            var newMember = oldMember.JsonClone();
            var oldMembersNotMatched = new List<MemberDefinition>();
            var newMembersNotMatched = new List<MemberDefinition>();
            var matches = new List<MemberMatch>
            {
                new MemberMatch(oldMember, newMember)
            };

            var results = new MatchResults(matches, oldMembersNotMatched, newMembersNotMatched);
            var oldNodes = new List<SyntaxNode>();

            var newNodes = new List<SyntaxNode>();

            evaluator.CompareNodes(oldNodes, newNodes).Returns(results);

            var sut = new ChangeCalculator(evaluator, comparers, _logger);

            Action action = () => sut.CalculateChange(oldNodes, newNodes);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification =
            "Testing constructor guard clause")]
        public void ThrowsExceptionWhenCreatedWithEmptyComparers()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparers = new List<IMemberComparer>();

            Action action = () => new ChangeCalculator(evaluator, comparers, _logger);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification =
            "Testing constructor guard clause")]
        public void ThrowsExceptionWhenCreatedWithNullComparers()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();

            Action action = () => new ChangeCalculator(evaluator, null, _logger);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification =
            "Testing constructor guard clause")]
        public void ThrowsExceptionWhenCreatedWithNullEvaluator()
        {
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};

            Action action = () => new ChangeCalculator(null, comparers, _logger);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification =
            "Testing constructor guard clause")]
        public void ThrowsExceptionWhenCreatedWithNullLogger()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};

            Action action = () => new ChangeCalculator(evaluator, comparers, null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}