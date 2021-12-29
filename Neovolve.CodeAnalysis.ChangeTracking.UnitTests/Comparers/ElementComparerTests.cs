namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;
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
        
        [Fact]
        public void CompareMatchThrowsExceptionWithNullMatch()
        {
            var firstItem = Substitute.For<IClassDefinition>();
            var secondItem = Substitute.For<IClassDefinition>();
            var result = new ComparisonResult(SemVerChangeType.Breaking, firstItem, secondItem,
                Guid.NewGuid().ToString());
            var options = TestComparerOptions.Default;

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