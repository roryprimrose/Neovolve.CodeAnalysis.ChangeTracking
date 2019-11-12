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

    public class MatchEvaluatorTests
    {
        private readonly ILogger _logger;

        public MatchEvaluatorTests(ITestOutputHelper output)
        {
            _logger = output.BuildLogger();
        }

        [Fact]
        public void CompareNodesReturnsEmptyResultsWhenNoNodesToEvaluate()
        {
            var scanner = Substitute.For<INodeScanner>();
            var matcher = Substitute.For<IMemberMatcher>();
            var matches = new List<IMemberMatcher> {matcher};
            var oldNodes = new List<SyntaxNode>();
            var newNodes = new List<SyntaxNode>();

            var sut = new MatchEvaluator(scanner, matches, _logger);

            var actual = sut.CompareNodes(oldNodes, newNodes);

            actual.Matches.Should().BeEmpty();
            actual.NewMembersNotMatched.Should().BeEmpty();
            actual.OldMembersNotMatched.Should().BeEmpty();
        }

        [Fact]
        public async Task CompareNodesReturnsMatch()
        {
            var scanner = Substitute.For<INodeScanner>();
            var matcher = Substitute.For<IMemberMatcher>();

            var member = Model.UsingModule<CompilerModule>().Create<PropertyDefinition>();
            var oldMembers = new List<MemberDefinition> {member};
            var newMembers = new List<MemberDefinition> {member};
            var matches = new List<IMemberMatcher> {matcher};
            var match = new MemberMatch(member, member);
            var oldNodes = new List<SyntaxNode>
            {
                await TestNode.Parse(TestNode.StandardProperty).ConfigureAwait(false)
            };
            var newNodes = new List<SyntaxNode>
            {
                await TestNode.Parse(TestNode.StandardProperty).ConfigureAwait(false)
            };

            scanner.FindDefinitions(oldNodes).Returns(oldMembers);
            scanner.FindDefinitions(newNodes).Returns(newMembers);
            matcher.IsSupported(member).Returns(true);
            matcher.GetMatch(member, member).Returns(match);

            var sut = new MatchEvaluator(scanner, matches, _logger);

            var actual = sut.CompareNodes(oldNodes, newNodes);

            actual.Matches.Should().HaveCount(1);
            actual.NewMembersNotMatched.Should().BeEmpty();
            actual.OldMembersNotMatched.Should().BeEmpty();
        }

        [Fact]
        public async Task CompareNodesReturnsMatchWithNewMemberNotMatched()
        {
            var scanner = Substitute.For<INodeScanner>();
            var matcher = Substitute.For<IMemberMatcher>();

            var member = Model.Create<PropertyDefinition>();
            var memberNotMatched = Model.Create<MemberDefinition>();
            var oldMembers = new List<MemberDefinition> {member};
            var newMembers = new List<MemberDefinition> {member, memberNotMatched};
            var matches = new List<IMemberMatcher> {matcher};
            var match = new MemberMatch(member, member);
            var oldNodes = new List<SyntaxNode>
            {
                await TestNode.Parse(TestNode.StandardProperty).ConfigureAwait(false)
            };
            var newNodes = new List<SyntaxNode>
            {
                await TestNode.Parse(TestNode.StandardProperty).ConfigureAwait(false)
            };

            scanner.FindDefinitions(oldNodes).Returns(oldMembers);
            scanner.FindDefinitions(newNodes).Returns(newMembers);
            matcher.IsSupported(Arg.Any<MemberDefinition>()).Returns(true);
            matcher.GetMatch(member, member).Returns(match);

            var sut = new MatchEvaluator(scanner, matches, _logger);

            var actual = sut.CompareNodes(oldNodes, newNodes);

            actual.Matches.Should().HaveCount(1);
            actual.NewMembersNotMatched.Should().HaveCount(1);
            actual.NewMembersNotMatched.Should().Contain(memberNotMatched);
            actual.OldMembersNotMatched.Should().BeEmpty();
        }

        [Fact]
        public async Task CompareNodesReturnsMatchWithOldMemberNotMatched()
        {
            var scanner = Substitute.For<INodeScanner>();
            var matcher = Substitute.For<IMemberMatcher>();

            var member = Model.Create<PropertyDefinition>();
            var memberNotMatched = Model.Create<MemberDefinition>();
            var oldMembers = new List<MemberDefinition> {member, memberNotMatched};
            var newMembers = new List<MemberDefinition> {member};
            var matches = new List<IMemberMatcher> {matcher};
            var match = new MemberMatch(member, member);
            var oldNodes = new List<SyntaxNode>
            {
                await TestNode.Parse(TestNode.StandardProperty).ConfigureAwait(false)
            };
            var newNodes = new List<SyntaxNode>
            {
                await TestNode.Parse(TestNode.StandardProperty).ConfigureAwait(false)
            };

            scanner.FindDefinitions(oldNodes).Returns(oldMembers);
            scanner.FindDefinitions(newNodes).Returns(newMembers);
            matcher.IsSupported(Arg.Any<MemberDefinition>()).Returns(true);
            matcher.GetMatch(member, member).Returns(match);

            var sut = new MatchEvaluator(scanner, matches, _logger);

            var actual = sut.CompareNodes(oldNodes, newNodes);

            actual.Matches.Should().HaveCount(1);
            actual.NewMembersNotMatched.Should().BeEmpty();
            actual.OldMembersNotMatched.Should().HaveCount(1);
            actual.OldMembersNotMatched.Should().Contain(memberNotMatched);
        }

        [Fact]
        public async Task CompareNodesReturnsSkipsProcessingNewMemberWhenNoMatcherNotSupported()
        {
            var scanner = Substitute.For<INodeScanner>();
            var matcher = Substitute.For<IMemberMatcher>();

            var member = Model.Create<PropertyDefinition>();
            var memberNotMatched = Model.Create<MemberDefinition>();
            var oldMembers = new List<MemberDefinition> {member};
            var newMembers = new List<MemberDefinition> {member, memberNotMatched };
            var matches = new List<IMemberMatcher> {matcher};
            var match = new MemberMatch(member, member);
            var oldNodes = new List<SyntaxNode>
            {
                await TestNode.Parse(TestNode.StandardProperty).ConfigureAwait(false)
            };
            var newNodes = new List<SyntaxNode>
            {
                await TestNode.Parse(TestNode.StandardProperty).ConfigureAwait(false)
            };

            scanner.FindDefinitions(oldNodes).Returns(oldMembers);
            scanner.FindDefinitions(newNodes).Returns(newMembers);
            matcher.IsSupported(member).Returns(true);
            matcher.GetMatch(member, member).Returns(match);

            var sut = new MatchEvaluator(scanner, matches, _logger);

            var actual = sut.CompareNodes(oldNodes, newNodes);

            actual.Matches.Should().HaveCount(1);
            actual.NewMembersNotMatched.Should().HaveCount(1);
            actual.NewMembersNotMatched.Should().Contain(memberNotMatched);
            actual.OldMembersNotMatched.Should().BeEmpty();
        }

        [Fact]
        public async Task CompareNodesThrowsExceptionWhenNoSupportingMatcherFound()
        {
            var scanner = Substitute.For<INodeScanner>();
            var matcher = Substitute.For<IMemberMatcher>();

            var member = Model.Create<PropertyDefinition>();
            var memberNotMatched = Model.Create<MemberDefinition>();
            var oldMembers = new List<MemberDefinition> {member, memberNotMatched};
            var newMembers = new List<MemberDefinition> {member};
            var matches = new List<IMemberMatcher> {matcher};
            var oldNodes = new List<SyntaxNode>
            {
                await TestNode.Parse(TestNode.StandardProperty).ConfigureAwait(false)
            };
            var newNodes = new List<SyntaxNode>
            {
                await TestNode.Parse(TestNode.StandardProperty).ConfigureAwait(false)
            };

            scanner.FindDefinitions(oldNodes).Returns(oldMembers);
            scanner.FindDefinitions(newNodes).Returns(newMembers);

            var sut = new MatchEvaluator(scanner, matches, _logger);

            Action action = () => sut.CompareNodes(oldNodes, newNodes);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public async Task CompareNodesThrowsExceptionWithNullNewNodes()
        {
            var scanner = Substitute.For<INodeScanner>();
            var matcher = Substitute.For<IMemberMatcher>();
            var matches = new List<IMemberMatcher> {matcher};
            var oldNodes = new List<SyntaxNode>
            {
                await TestNode.Parse(TestNode.StandardProperty).ConfigureAwait(false)
            };

            var sut = new MatchEvaluator(scanner, matches, _logger);

            Action action = () => sut.CompareNodes(oldNodes, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task CompareNodesThrowsExceptionWithNullOldNodes()
        {
            var scanner = Substitute.For<INodeScanner>();
            var matcher = Substitute.For<IMemberMatcher>();
            var matches = new List<IMemberMatcher> {matcher};
            var newNodes = new List<SyntaxNode>
            {
                await TestNode.Parse(TestNode.StandardProperty).ConfigureAwait(false)
            };

            var sut = new MatchEvaluator(scanner, matches, _logger);

            Action action = () => sut.CompareNodes(null, newNodes);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing constructor guard clause")]
        public void ThrowsExceptionWhenCreatedWithEmptyMatches()
        {
            var scanner = Substitute.For<INodeScanner>();

            var matches = new List<IMemberMatcher>();

            Action action = () => new MatchEvaluator(scanner, matches, _logger);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing constructor guard clause")]
        public void ThrowsExceptionWhenCreatedWithNullLogger()
        {
            var scanner = Substitute.For<INodeScanner>();
            var matcher = Substitute.For<IMemberMatcher>();
            var matches = new List<IMemberMatcher> {matcher};

            Action action = () => new MatchEvaluator(scanner, matches, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing constructor guard clause")]
        public void ThrowsExceptionWhenCreatedWithNullMatches()
        {
            var scanner = Substitute.For<INodeScanner>();

            Action action = () => new MatchEvaluator(scanner, null, _logger);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing constructor guard clause")]
        public void ThrowsExceptionWhenCreatedWithNullScanner()
        {
            var matcher = Substitute.For<IMemberMatcher>();
            var matches = new List<IMemberMatcher> {matcher};

            Action action = () => new MatchEvaluator(null, matches, _logger);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}