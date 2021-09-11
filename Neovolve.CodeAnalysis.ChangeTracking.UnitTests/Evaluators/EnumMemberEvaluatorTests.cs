namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Evaluators
{
    using System.Linq;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;

    public class EnumMemberEvaluatorTests : Tests<EnumMemberEvaluator>
    {
        [Fact]
        public void FindMatchesIdentifiesEnumMembersMatchingOnIndex()
        {
            var oldEnumMember = new TestEnumMemberDefinition();
            var newEnumMember = new TestEnumMemberDefinition {Index = oldEnumMember.Index};
            var oldEnumMembers = new[]
            {
                oldEnumMember
            };
            var newEnumMembers = new[]
            {
                newEnumMember
            };

            var sut = new EnumMemberEvaluator();

            var results = sut.FindMatches(oldEnumMembers, newEnumMembers);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldEnumMember);
            results.MatchingItems.First().NewItem.Should().Be(newEnumMember);
            results.ItemsAdded.Should().BeEmpty();
            results.ItemsRemoved.Should().BeEmpty();
        }

        [Fact]
        public void FindMatchesIdentifiesEnumMembersMatchingOnName()
        {
            var oldEnumMember = new TestEnumMemberDefinition();
            var newEnumMember = new TestEnumMemberDefinition {Name = oldEnumMember.Name};
            var oldEnumMembers = new[]
            {
                oldEnumMember
            };
            var newEnumMembers = new[]
            {
                newEnumMember
            };

            var sut = new EnumMemberEvaluator();

            var results = sut.FindMatches(oldEnumMembers, newEnumMembers);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldEnumMember);
            results.MatchingItems.First().NewItem.Should().Be(newEnumMember);
            results.ItemsAdded.Should().BeEmpty();
            results.ItemsRemoved.Should().BeEmpty();
        }

        [Fact]
        public void FindMatchesIdentifiesEnumMembersMatchingOnValue()
        {
            var oldEnumMember = new TestEnumMemberDefinition();
            var newEnumMember = new TestEnumMemberDefinition {Value = oldEnumMember.Value};
            var oldEnumMembers = new[]
            {
                oldEnumMember
            };
            var newEnumMembers = new[]
            {
                newEnumMember
            };

            var sut = new EnumMemberEvaluator();

            var results = sut.FindMatches(oldEnumMembers, newEnumMembers);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldEnumMember);
            results.MatchingItems.First().NewItem.Should().Be(newEnumMember);
            results.ItemsAdded.Should().BeEmpty();
            results.ItemsRemoved.Should().BeEmpty();
        }

        [Theory]
        [InlineData("123", "")]
        [InlineData("", "123")]
        [InlineData("234", "123")]
        public void FindMatchesIdentifiesEnumMembersNotMatching(string oldValue, string newValue)
        {
            var oldEnumMember = new TestEnumMemberDefinition {Value = oldValue};
            var newEnumMember = new TestEnumMemberDefinition {Value = newValue};
            var oldEnumMembers = new[]
            {
                oldEnumMember
            };
            var newEnumMembers = new[]
            {
                newEnumMember
            };

            var sut = new EnumMemberEvaluator();

            var results = sut.FindMatches(oldEnumMembers, newEnumMembers);

            results.MatchingItems.Should().BeEmpty();
            results.ItemsAdded.Should().HaveCount(1);
            results.ItemsAdded.First().Should().Be(newEnumMember);
            results.ItemsRemoved.Should().HaveCount(1);
            results.ItemsRemoved.First().Should().Be(oldEnumMember);
        }
    }
}