namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Evaluators
{
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;

    public class PropertyAccessorEvaluatorTests : Tests<PropertyAccessorEvaluator>
    {
        [Fact]
        public void FindMatchesIdentifiesPropertyAccessorsNotMatching()
        {
            var oldPropertyAccessor = new TestPropertyAccessorDefinition();
            var newPropertyAccessor = new TestPropertyAccessorDefinition();
            var oldMatchingPropertyAccessor = new TestPropertyAccessorDefinition();
            var oldPropertyAccessors = new[]
            {
                oldPropertyAccessor, oldMatchingPropertyAccessor
            };
            var newMatchingPropertyAccessor = new TestPropertyAccessorDefinition()
                .Set(x => x.Name = oldMatchingPropertyAccessor.Name);
            var newPropertyAccessors = new[]
            {
                newMatchingPropertyAccessor, newPropertyAccessor
            };
            
            var results = SUT.FindMatches(oldPropertyAccessors, newPropertyAccessors);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldMatchingPropertyAccessor);
            results.MatchingItems.First().NewItem.Should().Be(newMatchingPropertyAccessor);
            results.ItemsAdded.Should().HaveCount(1);
            results.ItemsAdded.First().Should().Be(newPropertyAccessor);
            results.ItemsRemoved.Should().HaveCount(1);
            results.ItemsRemoved.First().Should().Be(oldPropertyAccessor);
        }

        [Fact]
        public void FindMatchesReturnsSinglePropertyAccessorMatchingByName()
        {
            var oldPropertyAccessor = new TestPropertyAccessorDefinition();
            var oldPropertyAccessors = new[]
            {
                oldPropertyAccessor
            };
            var newPropertyAccessor = new TestPropertyAccessorDefinition()
                .Set(x => x.Name = oldPropertyAccessor.Name);
            var newPropertyAccessors = new[]
            {
                newPropertyAccessor
            };

            var sut = new PropertyAccessorEvaluator();

            var results = SUT.FindMatches(oldPropertyAccessors, newPropertyAccessors);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldPropertyAccessor);
            results.MatchingItems.First().NewItem.Should().Be(newPropertyAccessor);
            results.ItemsAdded.Should().BeEmpty();
            results.ItemsRemoved.Should().BeEmpty();
        }
    }
}