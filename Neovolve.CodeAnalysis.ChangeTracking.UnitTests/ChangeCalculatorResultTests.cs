namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class ChangeCalculatorResultTests
    {
        [Fact]
        public void AddDoesNotDowngradeChangeTypeWhenNewResultAddedWithLesserChangeType()
        {
            var firstMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var secondMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var firstResult = ComparisonResult.MemberRemoved(firstMember);
            var secondResult = ComparisonResult.MemberAdded(secondMember);

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
            var member = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var result = ComparisonResult.MemberAdded(member);

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
            var firstMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var secondMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var firstResult = ComparisonResult.MemberAdded(firstMember);
            var secondResult = ComparisonResult.MemberRemoved(secondMember);

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