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
            var oldMemberNotMatched = Model.UsingModule<ConfigurationModule>().Create<MemberDefinition>();
            var oldMembersNotMatched = new List<MemberDefinition> {oldMemberNotMatched};
            var newMembersNotMatched = new List<MemberDefinition>();
            var matches = new List<MemberMatch>();
            var results = new MatchResults(matches, oldMembersNotMatched, newMembersNotMatched);
            var oldNodes = new List<SyntaxNode>
            {
                await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false)
            };
            var newNodes = new List<SyntaxNode>();

            evaluator.CompareNodes(oldNodes, newNodes).Returns(results);

            var sut = new ChangeCalculator(evaluator, comparers, _logger);

            var actual = sut.CalculateChanges(oldNodes, newNodes);

            actual.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task CompareChangesReturnsBreakingWhenOldPublicMemberHasNoNewMemberMatchWithNullLogger()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};
            var oldMemberNotMatched = Model.UsingModule<ConfigurationModule>().Create<MemberDefinition>();
            var oldMembersNotMatched = new List<MemberDefinition> {oldMemberNotMatched};
            var newMembersNotMatched = new List<MemberDefinition>();
            var matches = new List<MemberMatch>();
            var results = new MatchResults(matches, oldMembersNotMatched, newMembersNotMatched);
            var oldNodes = new List<SyntaxNode>
            {
                await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false)
            };
            var newNodes = new List<SyntaxNode>();

            evaluator.CompareNodes(oldNodes, newNodes).Returns(results);

            var sut = new ChangeCalculator(evaluator, comparers, null);

            var actual = sut.CalculateChanges(oldNodes, newNodes);

            actual.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public void CompareChangesReturnsBreakingWhenSubsequentMatchesFindWorseChangeTypes()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<MemberDefinition>();
            var newMember = oldMember.JsonClone();
            var oldMembersNotMatched = new List<MemberDefinition>();
            var newMembersNotMatched = new List<MemberDefinition>();
            var matches = new List<MemberMatch>
            {
                new MemberMatch(oldMember, newMember),
                new MemberMatch(oldMember, newMember),
                new MemberMatch(oldMember, newMember)
            };
            var match = new MemberMatch(oldMember, newMember);
            var firstResult = ComparisonResult.NoChange(match);
            var secondResult = ComparisonResult.MemberChanged(SemVerChangeType.Feature, match, "feature change");
            var thirdResult = ComparisonResult.MemberChanged(SemVerChangeType.Breaking, match, "breaking change");

            var results = new MatchResults(matches, oldMembersNotMatched, newMembersNotMatched);
            var oldNodes = new List<SyntaxNode>();

            var newNodes = new List<SyntaxNode>();

            evaluator.CompareNodes(oldNodes, newNodes).Returns(results);
            comparer.IsSupported(Arg.Any<MemberDefinition>()).Returns(true);
            comparer.Compare(Arg.Any<MemberMatch>()).Returns(firstResult, secondResult, thirdResult);

            var sut = new ChangeCalculator(evaluator, comparers, _logger);

            var actual = sut.CalculateChanges(oldNodes, newNodes);

            actual.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Theory]
        [InlineData(SemVerChangeType.None)]
        [InlineData(SemVerChangeType.Feature)]
        [InlineData(SemVerChangeType.Breaking)]
        public void CompareChangesReturnsComparerValueForMemberMatch(SemVerChangeType changeType)
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<MemberDefinition>();
            var newMember = oldMember.JsonClone();
            var match = new MemberMatch(oldMember, newMember);
            var oldMembersNotMatched = new List<MemberDefinition>();
            var newMembersNotMatched = new List<MemberDefinition>();
            var matches = new List<MemberMatch>
            {
                new MemberMatch(oldMember, newMember)
            };
            ComparisonResult result;

            if (changeType == SemVerChangeType.None)
            {
                result = ComparisonResult.NoChange(match);
            }
            else
            {
                result = ComparisonResult.MemberChanged(changeType, match, changeType + " change");
            }

            var results = new MatchResults(matches, oldMembersNotMatched, newMembersNotMatched);
            var oldNodes = new List<SyntaxNode>();

            var newNodes = new List<SyntaxNode>();

            evaluator.CompareNodes(oldNodes, newNodes).Returns(results);
            comparer.IsSupported(oldMember).Returns(true);
            comparer.IsSupported(newMember).Returns(true);
            comparer.Compare(matches[0]).Returns(result);

            var sut = new ChangeCalculator(evaluator, comparers, _logger);

            var actual = sut.CalculateChanges(oldNodes, newNodes);

            actual.ChangeType.Should().Be(changeType);
        }

        [Theory]
        [InlineData(SemVerChangeType.None)]
        [InlineData(SemVerChangeType.Feature)]
        [InlineData(SemVerChangeType.Breaking)]
        public void CompareChangesReturnsComparerValueForMemberMatchWithNullLogger(SemVerChangeType changeType)
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<MemberDefinition>();
            var newMember = oldMember.JsonClone();
            var match = new MemberMatch(oldMember, newMember);
            var oldMembersNotMatched = new List<MemberDefinition>();
            var newMembersNotMatched = new List<MemberDefinition>();
            var matches = new List<MemberMatch>
            {
                new MemberMatch(oldMember, newMember)
            };
            ComparisonResult result;

            if (changeType == SemVerChangeType.None)
            {
                result = ComparisonResult.NoChange(match);
            }
            else
            {
                result = ComparisonResult.MemberChanged(changeType, match, changeType + " change");
            }

            var results = new MatchResults(matches, oldMembersNotMatched, newMembersNotMatched);
            var oldNodes = new List<SyntaxNode>();

            var newNodes = new List<SyntaxNode>();

            evaluator.CompareNodes(oldNodes, newNodes).Returns(results);
            comparer.IsSupported(oldMember).Returns(true);
            comparer.IsSupported(newMember).Returns(true);
            comparer.Compare(matches[0]).Returns(result);

            var sut = new ChangeCalculator(evaluator, comparers, null);

            var actual = sut.CalculateChanges(oldNodes, newNodes);

            actual.ChangeType.Should().Be(changeType);
        }

        [Fact]
        public async Task CompareChangesReturnsFeatureWhenNewPublicMemberHasNoOldMemberMatchAndNoMatchesFound()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};
            var oldMembersNotMatched = new List<MemberDefinition>();
            var newMemberNotMatched = Model.UsingModule<ConfigurationModule>().Create<MemberDefinition>();
            var newMembersNotMatched = new List<MemberDefinition> {newMemberNotMatched};
            var matches = new List<MemberMatch>();
            var results = new MatchResults(matches, oldMembersNotMatched, newMembersNotMatched);
            var oldNodes = new List<SyntaxNode>
            {
                await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false)
            };
            var newNodes = new List<SyntaxNode>();

            evaluator.CompareNodes(oldNodes, newNodes).Returns(results);

            var sut = new ChangeCalculator(evaluator, comparers, _logger);

            var actual = sut.CalculateChanges(oldNodes, newNodes);

            actual.ChangeType.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public async Task
            CompareChangesReturnsFeatureWhenNewPublicMemberHasNoOldMemberMatchAndNoMatchesFoundWithNullLogger()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};
            var oldMembersNotMatched = new List<MemberDefinition>();
            var newMemberNotMatched = Model.UsingModule<ConfigurationModule>().Create<MemberDefinition>();
            var newMembersNotMatched = new List<MemberDefinition> {newMemberNotMatched};
            var matches = new List<MemberMatch>();
            var results = new MatchResults(matches, oldMembersNotMatched, newMembersNotMatched);
            var oldNodes = new List<SyntaxNode>
            {
                await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false)
            };
            var newNodes = new List<SyntaxNode>();

            evaluator.CompareNodes(oldNodes, newNodes).Returns(results);

            var sut = new ChangeCalculator(evaluator, comparers, null);

            var actual = sut.CalculateChanges(oldNodes, newNodes);

            actual.ChangeType.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public void CompareChangesReturnsNoneWhenEmptyComparisonReturned()
        {
            var matches = Array.Empty<MemberMatch>();
            var emptyMissingMembers = Array.Empty<MemberDefinition>();
            var matchResults = new MatchResults(matches, emptyMissingMembers, emptyMissingMembers);
            var oldNodes = new List<SyntaxNode>();
            var newNodes = new List<SyntaxNode>();

            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};

            evaluator.CompareNodes(oldNodes, newNodes).Returns(matchResults);

            var sut = new ChangeCalculator(evaluator, comparers, _logger);

            var actual = sut.CalculateChanges(oldNodes, newNodes);

            actual.ChangeType.Should().Be(SemVerChangeType.None);
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

            var actual = sut.CalculateChanges(oldNodes, newNodes);

            actual.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Fact]
        public void CompareChangesThrowsExceptionWhenCreatedWithNullNewNodes()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};
            var oldNodes = new List<SyntaxNode>();

            var sut = new ChangeCalculator(evaluator, comparers, _logger);

            Action action = () => sut.CalculateChanges(oldNodes, null!);

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

            Action action = () => sut.CalculateChanges(null!, newNodes);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CompareChangesThrowsExceptionWhenNoComparerFoundForMember()
        {
            var evaluator = Substitute.For<IMatchEvaluator>();
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<MemberDefinition>();
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

            Action action = () => sut.CalculateChanges(oldNodes, newNodes);

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

            Action action = () => new ChangeCalculator(evaluator, null!, _logger);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification =
            "Testing constructor guard clause")]
        public void ThrowsExceptionWhenCreatedWithNullEvaluator()
        {
            var comparer = Substitute.For<IMemberComparer>();
            var comparers = new List<IMemberComparer> {comparer};

            Action action = () => new ChangeCalculator(null!, comparers, _logger);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}