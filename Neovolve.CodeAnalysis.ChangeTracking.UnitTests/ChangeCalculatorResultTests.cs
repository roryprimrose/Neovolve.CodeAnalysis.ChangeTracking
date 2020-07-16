namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xunit;

    public class ChangeCalculatorResultTests
    {
        [Fact]
        public void AddDoesNotDowngradeChangeTypeWhenNewResultAddedWithLesserChangeType()
        {
            var firstResult = new ComparisonResult(SemVerChangeType.Breaking, null, null, Guid.NewGuid().ToString());
            var secondResult = new ComparisonResult(SemVerChangeType.Feature, null, null, Guid.NewGuid().ToString());

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
            var result = new ComparisonResult(SemVerChangeType.Feature, null, null, Guid.NewGuid().ToString());

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
            var firstResult = new ComparisonResult(SemVerChangeType.Feature, null, null, Guid.NewGuid().ToString());
            var secondResult = new ComparisonResult(SemVerChangeType.Breaking, null, null, Guid.NewGuid().ToString());

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