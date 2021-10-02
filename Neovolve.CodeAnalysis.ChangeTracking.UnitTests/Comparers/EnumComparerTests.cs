namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
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

        [Theory]
        [InlineData(SemVerChangeType.None)]
        [InlineData(SemVerChangeType.Feature)]
        [InlineData(SemVerChangeType.Breaking)]
        public void CompareMatchCalculatesResultFromUnderlyingTypeChangeTable(SemVerChangeType expected)
        {
            var oldItem = new TestEnumDefinition().Set(x => x.ImplementedTypes = new[] { "byte" });
            var newItem = oldItem.JsonClone().Set(x => x.ImplementedTypes = new[] { "ushort" });
            var match = new ItemMatch<IEnumDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            Service<IEnumUnderlyingTypeChangeTable>()
                .CalculateChange(oldItem.ImplementedTypes.First(),
                    newItem.ImplementedTypes.First()).Returns(expected);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            if (expected == SemVerChangeType.None)
            {
                actual.Should().BeEmpty();
            }
            else
            {
                actual.Should().HaveCount(1);
                actual[0].ChangeType.Should().Be(expected);
            }
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenNamespaceChanged()
        {
            var oldItem = new TestEnumDefinition();
            var newItem = oldItem.JsonClone().Set(x => x.Namespace = Guid.NewGuid().ToString());
            var match = new ItemMatch<IEnumDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

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
            var options = TestComparerOptions.Default;
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

        [Theory]
        [InlineData("", "", SemVerChangeType.None)]
        [InlineData("", "short", SemVerChangeType.Breaking)]
        [InlineData("short", "", SemVerChangeType.Breaking)]
        [InlineData("short", "long", SemVerChangeType.Breaking)]
        public void CompareMatchReturnsResultFromEnumUnderlyingTypeChangeTable(string oldValue, string newValue,
            SemVerChangeType expected)
        {
            var oldItem = new TestEnumDefinition();
            var newItem = oldItem.JsonClone();

            if (string.IsNullOrWhiteSpace(oldValue) == false)
            {
                oldItem.ImplementedTypes = new[] { oldValue };
            }

            if (string.IsNullOrWhiteSpace(newValue) == false)
            {
                newItem.ImplementedTypes = new[] { newValue };
            }

            var match = new ItemMatch<IEnumDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            Service<IEnumUnderlyingTypeChangeTable>().CalculateChange(oldValue, newValue).Returns(expected);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            if (expected == SemVerChangeType.None)
            {
                actual.Should().BeEmpty();
            }
            else
            {
                actual.Should().HaveCount(1);

                actual[0].ChangeType.Should().Be(expected);
            }
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
        public void ThrowsExceptionWhenCreatedWithNullEnumAccessModifierComparer()
        {
            var enumAccessModifiersComparer = Substitute.For<IEnumMemberMatchProcessor>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();
            var underlyingTypeChangeTable = Substitute.For<IEnumUnderlyingTypeChangeTable>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () =>
                new EnumComparer(enumAccessModifiersComparer, null!, underlyingTypeChangeTable, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullEnumMemberProcessor()
        {
            var accessModifiersComparer = Substitute.For<IEnumAccessModifiersComparer>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();
            var underlyingTypeChangeTable = Substitute.For<IEnumUnderlyingTypeChangeTable>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () =>
                new EnumComparer(null!, accessModifiersComparer, underlyingTypeChangeTable, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullUnderlyingTypeChangeTable()
        {
            var enumAccessModifiersComparer = Substitute.For<IEnumMemberMatchProcessor>();
            var accessModifiersComparer = Substitute.For<IEnumAccessModifiersComparer>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () =>
                new EnumComparer(enumAccessModifiersComparer, accessModifiersComparer, null!, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}