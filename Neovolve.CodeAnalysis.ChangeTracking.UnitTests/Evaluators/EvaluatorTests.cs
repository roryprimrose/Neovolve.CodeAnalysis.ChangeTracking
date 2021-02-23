namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Evaluators
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class EvaluatorTests
    {
        private readonly ILogger _logger;

        public EvaluatorTests(ITestOutputHelper output)
        {
            _logger = output.BuildLogger();
        }

        //private class Wrapper<T> : Evaluator<T> where T : IItemDefinition
        //{
        //    public IMatchResults<T> RunFindMatches(IMatchResults<T> results, Func<T, T, bool> evaluator)
        //    {
        //        return FindMatches(results, evaluator);
        //    }

        //    public override IMatchResults<T> FindMatches(IEnumerable<T> oldItems, IEnumerable<T> newItems)
        //    {
        //        return null!;
        //    }
        //}

        //[Fact]
        //public void CompareNodesReturnsEmptyResultsWhenNoNodesToEvaluate()
        //{
        //    var oldNodes = new List<SyntaxNode>();
        //    var newNodes = new List<SyntaxNode>();

        //    var sut = new Wrapper<IClassDefinition>();

        //    var actual = sut.RunFindMatches(oldNodes, newNodes);

        //    actual.MatchingItems.Should().BeEmpty();
        //    actual.ItemsAdded.Should().BeEmpty();
        //    actual.ItemsRemoved.Should().BeEmpty();
        //}

        //[Fact]
        //public async Task CompareNodesReturnsMatch()
        //{
        //    var scanner = Substitute.For<INodeScanner>();
        //    var matcher = Substitute.For<IMemberMatcher>();

        //    var member = Model.UsingModule<ConfigurationModule>().Create<OldPropertyDefinition>();
        //    var oldMembers = new List<OldMemberDefinition> { member };
        //    var newMembers = new List<OldMemberDefinition> { member };
        //    var matches = new List<IMemberMatcher> { matcher };
        //    var match = new ItemMatch<>(member, member);
        //    var oldNodes = new List<SyntaxNode>
        //    {
        //        await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false)
        //    };
        //    var newNodes = new List<SyntaxNode>
        //    {
        //        await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false)
        //    };

        //    scanner.FindDefinitions(oldNodes).Returns(oldMembers);
        //    scanner.FindDefinitions(newNodes).Returns(newMembers);
        //    matcher.IsSupported(member).Returns(true);
        //    matcher.GetMatch(member, member).Returns(match);

        //    var sut = new Evaluator(scanner, matches, _logger);

        //    var actual = sut.FindMatches(oldNodes, newNodes);

        //    actual.MatchingItems.Should().HaveCount(1);
        //    actual.ItemsAdded.Should().BeEmpty();
        //    actual.ItemsRemoved.Should().BeEmpty();
        //}

        //[Fact]
        //public async Task CompareNodesReturnsMatchWithNewMemberNotMatched()
        //{
        //    var scanner = Substitute.For<INodeScanner>();
        //    var matcher = Substitute.For<IMemberMatcher>();

        //    var member = Model.UsingModule<ConfigurationModule>().Create<OldPropertyDefinition>();
        //    var memberNotMatched = Model.UsingModule<ConfigurationModule>().Create<OldMemberDefinition>();
        //    var oldMembers = new List<OldMemberDefinition> { member };
        //    var newMembers = new List<OldMemberDefinition> { member, memberNotMatched };
        //    var matches = new List<IMemberMatcher> { matcher };
        //    var match = new ItemMatch<>(member, member);
        //    var oldNodes = new List<SyntaxNode>
        //    {
        //        await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false)
        //    };
        //    var newNodes = new List<SyntaxNode>
        //    {
        //        await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false)
        //    };

        //    scanner.FindDefinitions(oldNodes).Returns(oldMembers);
        //    scanner.FindDefinitions(newNodes).Returns(newMembers);
        //    matcher.IsSupported(Arg.Any<OldMemberDefinition>()).Returns(true);
        //    matcher.GetMatch(member, member).Returns(match);

        //    var sut = new Evaluator(scanner, matches, _logger);

        //    var actual = sut.FindMatches(oldNodes, newNodes);

        //    actual.MatchingItems.Should().HaveCount(1);
        //    actual.ItemsAdded.Should().HaveCount(1);
        //    actual.ItemsAdded.Should().Contain(memberNotMatched);
        //    actual.ItemsRemoved.Should().BeEmpty();
        //}

        //[Fact]
        //public async Task CompareNodesReturnsMatchWithOldMemberNotMatched()
        //{
        //    var scanner = Substitute.For<INodeScanner>();
        //    var matcher = Substitute.For<IMemberMatcher>();

        //    var member = Model.UsingModule<ConfigurationModule>().Create<OldPropertyDefinition>();
        //    var memberNotMatched = Model.UsingModule<ConfigurationModule>().Create<OldMemberDefinition>();
        //    var oldMembers = new List<OldMemberDefinition> { member, memberNotMatched };
        //    var newMembers = new List<OldMemberDefinition> { member };
        //    var matches = new List<IMemberMatcher> { matcher };
        //    var match = new ItemMatch<>(member, member);
        //    var oldNodes = new List<SyntaxNode>
        //    {
        //        await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false)
        //    };
        //    var newNodes = new List<SyntaxNode>
        //    {
        //        await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false)
        //    };

        //    scanner.FindDefinitions(oldNodes).Returns(oldMembers);
        //    scanner.FindDefinitions(newNodes).Returns(newMembers);
        //    matcher.IsSupported(Arg.Any<OldMemberDefinition>()).Returns(true);
        //    matcher.GetMatch(member, member).Returns(match);

        //    var sut = new Evaluator(scanner, matches, _logger);

        //    var actual = sut.FindMatches(oldNodes, newNodes);

        //    actual.MatchingItems.Should().HaveCount(1);
        //    actual.ItemsAdded.Should().BeEmpty();
        //    actual.ItemsRemoved.Should().HaveCount(1);
        //    actual.ItemsRemoved.Should().Contain(memberNotMatched);
        //}

        //[Fact]
        //public async Task CompareNodesReturnsSkipsProcessingNewMemberWhenNoMatcherNotSupported()
        //{
        //    var scanner = Substitute.For<INodeScanner>();
        //    var matcher = Substitute.For<IMemberMatcher>();

        //    var member = Model.UsingModule<ConfigurationModule>().Create<OldPropertyDefinition>();
        //    var memberNotMatched = Model.UsingModule<ConfigurationModule>().Create<OldMemberDefinition>();
        //    var oldMembers = new List<OldMemberDefinition> { member };
        //    var newMembers = new List<OldMemberDefinition> { member, memberNotMatched };
        //    var matches = new List<IMemberMatcher> { matcher };
        //    var match = new ItemMatch<>(member, member);
        //    var oldNodes = new List<SyntaxNode>
        //    {
        //        await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false)
        //    };
        //    var newNodes = new List<SyntaxNode>
        //    {
        //        await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false)
        //    };

        //    scanner.FindDefinitions(oldNodes).Returns(oldMembers);
        //    scanner.FindDefinitions(newNodes).Returns(newMembers);
        //    matcher.IsSupported(member).Returns(true);
        //    matcher.GetMatch(member, member).Returns(match);

        //    var sut = new Evaluator(scanner, matches, _logger);

        //    var actual = sut.FindMatches(oldNodes, newNodes);

        //    actual.MatchingItems.Should().HaveCount(1);
        //    actual.ItemsAdded.Should().HaveCount(1);
        //    actual.ItemsAdded.Should().Contain(memberNotMatched);
        //    actual.ItemsRemoved.Should().BeEmpty();
        //}

        //[Fact]
        //public async Task CompareNodesSupportsNullLogger()
        //{
        //    var scanner = Substitute.For<INodeScanner>();
        //    var matcher = Substitute.For<IMemberMatcher>();

        //    var member = Model.UsingModule<ConfigurationModule>().Create<OldPropertyDefinition>();
        //    var oldMembers = new List<OldMemberDefinition> { member };
        //    var newMembers = new List<OldMemberDefinition> { member };
        //    var matches = new List<IMemberMatcher> { matcher };
        //    var match = new ItemMatch<>(member, member);
        //    var oldNodes = new List<SyntaxNode>
        //    {
        //        await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false)
        //    };
        //    var newNodes = new List<SyntaxNode>
        //    {
        //        await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false)
        //    };

        //    scanner.FindDefinitions(oldNodes).Returns(oldMembers);
        //    scanner.FindDefinitions(newNodes).Returns(newMembers);
        //    matcher.IsSupported(member).Returns(true);
        //    matcher.GetMatch(member, member).Returns(match);

        //    var sut = new Evaluator(scanner, matches, null);

        //    var actual = sut.FindMatches(oldNodes, newNodes);

        //    actual.MatchingItems.Should().HaveCount(1);
        //    actual.ItemsAdded.Should().BeEmpty();
        //    actual.ItemsRemoved.Should().BeEmpty();
        //}

        //[Fact]
        //public async Task CompareNodesThrowsExceptionWhenNoSupportingMatcherFound()
        //{
        //    var scanner = Substitute.For<INodeScanner>();
        //    var matcher = Substitute.For<IMemberMatcher>();

        //    var member = Model.UsingModule<ConfigurationModule>().Create<OldPropertyDefinition>();
        //    var memberNotMatched = Model.UsingModule<ConfigurationModule>().Create<OldMemberDefinition>();
        //    var oldMembers = new List<OldMemberDefinition> { member, memberNotMatched };
        //    var newMembers = new List<OldMemberDefinition> { member };
        //    var matches = new List<IMemberMatcher> { matcher };
        //    var oldNodes = new List<SyntaxNode>
        //    {
        //        await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false)
        //    };
        //    var newNodes = new List<SyntaxNode>
        //    {
        //        await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false)
        //    };

        //    scanner.FindDefinitions(oldNodes).Returns(oldMembers);
        //    scanner.FindDefinitions(newNodes).Returns(newMembers);

        //    var sut = new Evaluator(scanner, matches, _logger);

        //    Action action = () => sut.FindMatches(oldNodes, newNodes);

        //    action.Should().Throw<InvalidOperationException>();
        //}

        //[Fact]
        //public async Task CompareNodesThrowsExceptionWithNullNewNodes()
        //{
        //    var scanner = Substitute.For<INodeScanner>();
        //    var matcher = Substitute.For<IMemberMatcher>();
        //    var matches = new List<IMemberMatcher> { matcher };
        //    var oldNodes = new List<SyntaxNode>
        //    {
        //        await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false)
        //    };

        //    var sut = new Evaluator(scanner, matches, _logger);

        //    Action action = () => sut.FindMatches(oldNodes, null!);

        //    action.Should().Throw<ArgumentNullException>();
        //}

        //[Fact]
        //public async Task CompareNodesThrowsExceptionWithNullOldNodes()
        //{
        //    var scanner = Substitute.For<INodeScanner>();
        //    var matcher = Substitute.For<IMemberMatcher>();
        //    var matches = new List<IMemberMatcher> { matcher };
        //    var newNodes = new List<SyntaxNode>
        //    {
        //        await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false)
        //    };

        //    var sut = new Evaluator(scanner, matches, _logger);

        //    Action action = () => sut.FindMatches(null!, newNodes);

        //    action.Should().Throw<ArgumentNullException>();
        //}

        //[Fact]
        //[SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification =
        //    "The constructor is the target of the test")]
        //public void ThrowsExceptionWhenCreatedWithEmptyMatches()
        //{
        //    var scanner = Substitute.For<INodeScanner>();

        //    var matches = new List<IMemberMatcher>();

        //    Action action = () => new Evaluator(scanner, matches, _logger);

        //    action.Should().Throw<ArgumentException>();
        //}

        //[Fact]
        //[SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification =
        //    "The constructor is the target of the test")]
        //public void ThrowsExceptionWhenCreatedWithNullMatches()
        //{
        //    var scanner = Substitute.For<INodeScanner>();

        //    Action action = () => new Evaluator(scanner, null!, _logger);

        //    action.Should().Throw<ArgumentNullException>();
        //}

        //[Fact]
        //[SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification =
        //    "The constructor is the target of the test")]
        //public void ThrowsExceptionWhenCreatedWithNullScanner()
        //{
        //    var matcher = Substitute.For<IMemberMatcher>();
        //    var matches = new List<IMemberMatcher> { matcher };

        //    Action action = () => new Evaluator(null!, matches, _logger);

        //    action.Should().Throw<ArgumentNullException>();
        //}
    }
}