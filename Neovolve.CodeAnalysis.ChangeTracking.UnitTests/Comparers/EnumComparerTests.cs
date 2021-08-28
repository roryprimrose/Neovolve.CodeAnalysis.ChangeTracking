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

    public class EnumComparerTests : Tests<EnumComparer>
    {
        private readonly ITestOutputHelper _output;

        public EnumComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenNamespaceChanged()
        {
            var oldItem = new TestEnumDefinition();
            var newItem = oldItem.JsonClone().Set(x => x.Namespace = Guid.NewGuid().ToString());
            var match = new ItemMatch<IEnumDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CompareMatchReturnsResultFromAccessModifiersComparer()
        {
            var oldItem = new TestEnumDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IEnumDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] { result };

            Service<IEnumAccessModifiersComparer>()
                .CompareMatch(
                    Arg.Is<ItemMatch<IAccessModifiersElement<EnumAccessModifiers>>>(
                        x => x.OldItem == oldItem && x.NewItem == newItem),
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void CompareMatchReturnsResultsFromEnumMemberProcessor()
        {
            var oldItem = new TestEnumDefinition().Set(x => x.Members = new[] { new TestEnumMemberDefinition() });
            var newItem = oldItem.JsonClone();
            var changeType = Model.Create<SemVerChangeType>();
            var match = new ItemMatch<IEnumDefinition>(oldItem, newItem);
            var options = Model.Create<ComparerOptions>();
            var result = new ComparisonResult(changeType, oldItem.Members.Last(), newItem.Members.Last(),
                Guid.NewGuid().ToString());
            var results = new List<ComparisonResult>
            {
                result
            };

            Service<IEnumMemberMatchProcessor>().CalculateChanges(oldItem.Members, newItem.Members, options)
                .Returns(results);

            var actual = SUT.CompareMatch(match, options);

            actual.Should().BeEquivalentTo(results);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullEnumMemberProcessor()
        {
            var accessModifiersComparer = Substitute.For<IEnumAccessModifiersComparer>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new EnumComparer(null!, accessModifiersComparer, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}