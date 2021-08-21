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

    public class BaseTypeComparerTests : Tests<BaseTypeComparer<IBaseTypeDefinition>>
    {
        private readonly ITestOutputHelper _output;

        public BaseTypeComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareMatchReturnsResultsFromAccessModifierComparer()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<IBaseTypeDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var result = new ComparisonResult(changeType, oldItem,
                newItem, Guid.NewGuid().ToString());
            var results = new List<ComparisonResult>
            {
                result
            };

            Service<IAccessModifiersComparer>()
                .CompareMatch(
                    Arg.Is<ItemMatch<IAccessModifiersElement<AccessModifiers>>>(x =>
                        x.OldItem == oldItem && x.NewItem == newItem), options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual[0].ChangeType.Should().Be(changeType);
            actual[0].Message.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullAccessModifiersComparer()
        {
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Wrapper(null!, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : BaseTypeComparer<IClassDefinition>
        {
            public Wrapper(IAccessModifiersComparer accessModifiersComparer,
                IAttributeMatchProcessor attributeProcessor) : base(
                accessModifiersComparer,
                attributeProcessor)
            {
            }
        }
    }
}