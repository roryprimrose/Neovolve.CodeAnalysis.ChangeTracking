namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class ChangeCalculatorResultTests
    {
        [Fact]
        public void AddDoesNotDowngradeChangeTypeWhenNewResultAddedWithLesserChangeType()
        {
            var firstMember = new TestPropertyDefinition();
            var secondMember = new TestPropertyDefinition();
            var firstResult = ComparisonResult.ItemRemoved(firstMember);
            var secondResult = ComparisonResult.ItemAdded(secondMember);

            var sut = new ChangeCalculatorResult();

            sut.Add(firstResult);
            sut.Add(secondResult);

            sut.ChangeType.Should().Be(firstResult.ChangeType);
            sut.ComparisonResults.Should().HaveCount(2);
            sut.ComparisonResults.First().Should().Be(firstResult);
            sut.ComparisonResults.Skip(1).First().Should().Be(secondResult);
        }

        [Fact]
        public void AddSetsChangeTypeToInitialResult()
        {
            var member = new TestPropertyDefinition();
            var result = ComparisonResult.ItemAdded(member);

            var sut = new ChangeCalculatorResult();

            sut.Add(result);

            sut.ChangeType.Should().Be(result.ChangeType);
            sut.ComparisonResults.Should().HaveCount(1);
            sut.ComparisonResults.First().Should().Be(result);
        }

        [Fact]
        public void AddThrowsExceptionWithNullResult()
        {
            var sut = new ChangeCalculatorResult();

            Action action = () => sut.Add(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddUpgradesChangeTypeWhenNewResultAddedWithGreaterChangeType()
        {
            var firstMember = new TestPropertyDefinition();
            var secondMember = new TestPropertyDefinition();
            var firstResult = ComparisonResult.ItemAdded(firstMember);
            var secondResult = ComparisonResult.ItemRemoved(secondMember);

            var sut = new ChangeCalculatorResult();

            sut.Add(firstResult);
            sut.Add(secondResult);

            sut.ChangeType.Should().Be(secondResult.ChangeType);
            sut.ComparisonResults.Should().HaveCount(2);
            sut.ComparisonResults.First().Should().Be(firstResult);
            sut.ComparisonResults.Skip(1).First().Should().Be(secondResult);
        }

        [Fact]
        public void CreatesWithDefaultProperties()
        {
            var actual = new ChangeCalculatorResult();

            actual.ComparisonResults.Should().BeEmpty();
            actual.ChangeType.Should().Be(SemVerChangeType.None);
        }
    }
}