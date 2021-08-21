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

    public class EnumComparerTests : Tests<EnumComparer>
    {
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
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            Action action = () => new EnumComparer(null!, accessModifiersComparer, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}