namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class ElementComparerTests
    {
        private readonly ITestOutputHelper _output;

        public ElementComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(false, false, SemVerChangeType.None, false)]
        [InlineData(true, false, SemVerChangeType.None, false)]
        [InlineData(false, true, SemVerChangeType.None, false)]
        [InlineData(true, true, SemVerChangeType.None, true)]
        [InlineData(false, false, SemVerChangeType.Feature, false)]
        [InlineData(true, false, SemVerChangeType.Feature, false)]
        [InlineData(false, true, SemVerChangeType.Feature, false)]
        [InlineData(true, true, SemVerChangeType.Feature, true)]
        [InlineData(false, false, SemVerChangeType.Breaking, false)]
        [InlineData(true, false, SemVerChangeType.Breaking, false)]
        [InlineData(false, true, SemVerChangeType.Breaking, false)]
        [InlineData(true, true, SemVerChangeType.Breaking, false)]
        public void CompareMatchEvaluatesAttributeMatchesWhenBothItemsVisibleAndItemMatchIsNotBreaking(
            bool firstItemVisible,
            bool secondItemVisible, SemVerChangeType changeType, bool attributesEvaluated)
        {
            var firstItem = Substitute.For<IClassDefinition>();
            var firstItemAttributes = Model.UsingModule<ConfigurationModule>().Create<List<TestAttributeDefinition>>();
            var secondItem = Substitute.For<IClassDefinition>();
            var secondItemAttributes = Model.UsingModule<ConfigurationModule>().Create<List<TestAttributeDefinition>>();
            var itemResult = new ComparisonResult(changeType, firstItem, secondItem, Guid.NewGuid().ToString());
            var attributeResult = new ComparisonResult(changeType, firstItemAttributes.Last(),
                secondItemAttributes.Last(), Guid.NewGuid().ToString());
            var attributeResults = new List<ComparisonResult> { attributeResult };
            var match = new ItemMatch<IClassDefinition>(firstItem, secondItem);
            var options = ComparerOptions.Default;

            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            firstItem.IsVisible.Returns(firstItemVisible);
            secondItem.IsVisible.Returns(secondItemVisible);
            attributeProcessor.CalculateChanges(firstItem.Attributes, secondItem.Attributes, options)
                .Returns(attributeResults);

            var sut = new Wrapper<IClassDefinition>(attributeProcessor, itemResult);

            var actual = sut.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

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
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(true, true, true)]
        public void CompareMatchEvaluatesMatchWhenBothItemsVisible(bool firstItemVisible,
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

            var actual = sut.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            if (matchEvaluated)
            {
                actual.Should().Contain(result);
            }
            else
            {
                actual.Should().NotContain(result);
            }
        }

        [Fact]
        public void CompareMatchThrowsExceptionWithNullMatch()
        {
            var firstItem = Substitute.For<IClassDefinition>();
            var secondItem = Substitute.For<IClassDefinition>();
            var result = new ComparisonResult(SemVerChangeType.Breaking, firstItem, secondItem,
                Guid.NewGuid().ToString());
            var options = ComparerOptions.Default;

            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            var sut = new Wrapper<IClassDefinition>(attributeProcessor, result);

            Action action = () => sut.CompareMatch(null!, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CompareMatchThrowsExceptionWithNullOptions()
        {
            var firstItem = Substitute.For<IClassDefinition>();
            var secondItem = Substitute.For<IClassDefinition>();
            var result = new ComparisonResult(SemVerChangeType.Breaking, firstItem, secondItem,
                Guid.NewGuid().ToString());
            var match = new ItemMatch<IClassDefinition>(firstItem, secondItem);

            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            var sut = new Wrapper<IClassDefinition>(attributeProcessor, result);

            Action action = () => sut.CompareMatch(match, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(false, false, null)]
        [InlineData(true, false, SemVerChangeType.Breaking)]
        [InlineData(false, true, SemVerChangeType.Feature)]
        public void CompareReturnsReturnsChangeTypeWhenAtLeastOneItemHidden(bool firstItemVisible,
            bool secondItemVisible, SemVerChangeType? changeType)
        {
            var oldMember = new TestClassDefinition()
                .Set(x => { x.IsVisible = firstItemVisible; });
            var newMember = oldMember.JsonClone()
                .Set(x => { x.IsVisible = secondItemVisible; });
            var match = new ItemMatch<IClassDefinition>(oldMember, newMember);
            var options = ComparerOptions.Default;

            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            var sut = new Wrapper<IClassDefinition>(attributeProcessor, null);

            var actual = sut.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            if (changeType == null)
            {
                actual.Should().BeEmpty();
            }
            else
            {
                actual.Should().HaveCount(1);
                actual[0].ChangeType.Should().Be(changeType);
            }
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

            protected override void EvaluateTypeDefinitionChanges(ItemMatch<T> match, ComparerOptions options, IChangeResultAggregator aggregator)
            {
                base.EvaluateTypeDefinitionChanges(match, options, aggregator);

                if (_matchResult != null)
                {
                    aggregator.AddResult(_matchResult);
                }
            }
        }
    }
}