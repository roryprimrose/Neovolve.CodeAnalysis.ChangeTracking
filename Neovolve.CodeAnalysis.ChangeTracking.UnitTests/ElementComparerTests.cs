namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;

    public class ElementComparerTests
    {
        [Theory]
        [InlineData(false, false, SemVerChangeType.None, false)]
        [InlineData(true, false, SemVerChangeType.None, true)]
        [InlineData(false, true, SemVerChangeType.None, true)]
        [InlineData(true, true, SemVerChangeType.None, true)]
        [InlineData(false, false, SemVerChangeType.Feature, false)]
        [InlineData(true, false, SemVerChangeType.Feature, true)]
        [InlineData(false, true, SemVerChangeType.Feature, true)]
        [InlineData(true, true, SemVerChangeType.Feature, true)]
        [InlineData(false, false, SemVerChangeType.Breaking, false)]
        [InlineData(true, false, SemVerChangeType.Breaking, false)]
        [InlineData(false, true, SemVerChangeType.Breaking, false)]
        [InlineData(true, true, SemVerChangeType.Breaking, false)]
        public void CompareItemsEvaluatesAttributeMatchesWhenAtLeastOneItemVisibleAndItemMatchIsNotBreaking(
            bool firstItemVisible,
            bool secondItemVisible, SemVerChangeType changeType, bool attributesEvaluated)
        {
            var firstItem = Substitute.For<IClassDefinition>();
            var firstItemAttributes = Model.Create<List<TestAttributeDefinition>>();
            var secondItem = Substitute.For<IClassDefinition>();
            var secondItemAttributes = Model.Create<List<TestAttributeDefinition>>();
            var itemResult = new ComparisonResult(changeType, firstItem, secondItem, Guid.NewGuid().ToString());
            var attributeResult = new ComparisonResult(changeType, firstItemAttributes[^1], secondItemAttributes[^1], Guid.NewGuid().ToString());
            var attributeResults = new List<ComparisonResult> {attributeResult};
            var match = new ItemMatch<IClassDefinition>(firstItem, secondItem);
            var options = ComparerOptions.Default;

            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            firstItem.IsVisible.Returns(firstItemVisible);
            secondItem.IsVisible.Returns(secondItemVisible);
            attributeProcessor.CalculateChanges(firstItem.Attributes, secondItem.Attributes, options)
                .Returns(attributeResults);

            var sut = new Wrapper<IClassDefinition>(attributeProcessor, itemResult);

            var actual = sut.CompareItems(match, options);

            if (attributesEvaluated)
            {
                attributeProcessor.Received().CalculateChanges(firstItem.Attributes, secondItem.Attributes, options);

                actual.Should().Contain(attributeResult);
            }
            else
            {
                attributeProcessor.DidNotReceive().CalculateChanges(Arg.Any<IEnumerable<IAttributeDefinition>>(),
                    Arg.Any<IEnumerable<IAttributeDefinition>>(), Arg.Any<ComparerOptions>());
            }
        }

        [Theory]
        [InlineData(false, false, false)]
        [InlineData(true, false, true)]
        [InlineData(false, true, true)]
        [InlineData(true, true, true)]
        public void CompareItemsEvaluatesMatchWhenAtLeastOneItemVisible(bool firstItemVisible,
            bool secondItemVisible, bool matchEvaluated)
        {
            var firstItem = Substitute.For<IClassDefinition>();
            var secondItem = Substitute.For<IClassDefinition>();
            var result = new ComparisonResult(SemVerChangeType.Breaking, firstItem, secondItem,
                Guid.NewGuid().ToString());
            var match = new ItemMatch<IClassDefinition>(firstItem, secondItem);
            var options = ComparerOptions.Default;

            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            firstItem.IsVisible.Returns(firstItemVisible);
            secondItem.IsVisible.Returns(secondItemVisible);

            var sut = new Wrapper<IClassDefinition>(attributeProcessor, result);

            var actual = sut.CompareItems(match, options);

            if (matchEvaluated)
            {
                actual.Should().Contain(result);
            }
            else
            {
                actual.Should().BeEmpty();
            }
        }

        [Fact]
        public void CompareItemsThrowsExceptionWithNullMatch()
        {
            var firstItem = Substitute.For<IClassDefinition>();
            var secondItem = Substitute.For<IClassDefinition>();
            var result = new ComparisonResult(SemVerChangeType.Breaking, firstItem, secondItem,
                Guid.NewGuid().ToString());
            var options = ComparerOptions.Default;

            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            var sut = new Wrapper<IClassDefinition>(attributeProcessor, result);

            Action action = () => sut.CompareItems(null!, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CompareItemsThrowsExceptionWithNullOptions()
        {
            var firstItem = Substitute.For<IClassDefinition>();
            var secondItem = Substitute.For<IClassDefinition>();
            var result = new ComparisonResult(SemVerChangeType.Breaking, firstItem, secondItem,
                Guid.NewGuid().ToString());
            var match = new ItemMatch<IClassDefinition>(firstItem, secondItem);

            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            var sut = new Wrapper<IClassDefinition>(attributeProcessor, result);

            Action action = () => sut.CompareItems(match, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullAttributeProcessor()
        {
            var firstItem = Substitute.For<IClassDefinition>();
            var secondItem = Substitute.For<IClassDefinition>();
            var result = new ComparisonResult(SemVerChangeType.Breaking, firstItem, secondItem,
                Guid.NewGuid().ToString());

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Wrapper<IClassDefinition>(null!, result);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper<T> : ElementComparer<T> where T : IElementDefinition
        {
            private readonly ComparisonResult? _matchResult;

            public Wrapper(IAttributeMatchProcessor attributeProcessor, ComparisonResult? matchResult) : base(
                attributeProcessor)
            {
                _matchResult = matchResult;
            }

            protected override void EvaluateMatch(ItemMatch<T> match, ComparerOptions options,
                IChangeResultAggregator aggregator)
            {
                if (_matchResult != null)
                {
                    aggregator.AddResult(_matchResult);
                }
            }
        }
    }
}