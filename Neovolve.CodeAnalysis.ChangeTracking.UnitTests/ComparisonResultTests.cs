namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;
    using Xunit.Abstractions;

    public class ComparisonResultTests
    {
        private readonly ITestOutputHelper _output;

        public ComparisonResultTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(true, SemVerChangeType.Feature)]
        [InlineData(false, SemVerChangeType.None)]
        public void MemberAddedPopulatesInformationFromParameters(bool isVisible, SemVerChangeType expected)
        {
            var newMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>()
                .Set(x => x.IsVisible = isVisible);

            var actual = ComparisonResult.MemberAdded(newMember);

            actual.Should().NotBeNull();

            _output.WriteLine(actual.Message);

            actual.ChangeType.Should().Be(expected);
            actual.OldMember.Should().BeNull();
            actual.NewMember.Should().Be(newMember);
            actual.Message.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void MemberAddedThrowsExceptionWithNullOldMember()
        {
            Action action = () => ComparisonResult.MemberAdded(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(SemVerChangeType.Feature)]
        [InlineData(SemVerChangeType.Breaking)]
        public void MemberChangedPopulatesInformationFromParameters(SemVerChangeType changeType)
        {
            var newMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var match = new MemberMatch(oldMember, newMember);
            var message = Guid.NewGuid().ToString();

            var actual = ComparisonResult.MemberChanged(changeType, match, message);

            actual.Should().NotBeNull();

            _output.WriteLine(actual.Message);

            actual.ChangeType.Should().Be(changeType);
            actual.OldMember.Should().Be(oldMember);
            actual.NewMember.Should().Be(newMember);
            actual.Message.Should().Be(message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void MemberChangedThrowsExceptionWithInvalidMessage(string message)
        {
            var newMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var match = new MemberMatch(oldMember, newMember);

            Action action = () =>
                ComparisonResult.MemberChanged(SemVerChangeType.Feature, match, message);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void MemberChangedThrowsExceptionWithNoneChangeType()
        {
            var newMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var match = new MemberMatch(oldMember, newMember);
            var message = Guid.NewGuid().ToString();

            Action action = () => ComparisonResult.MemberChanged(SemVerChangeType.None, match, message);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void MemberChangedThrowsExceptionWithNullMatch()
        {
            var message = Guid.NewGuid().ToString();

            Action action = () => ComparisonResult.MemberChanged(SemVerChangeType.Feature, null!, message);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(true, SemVerChangeType.Breaking)]
        [InlineData(false, SemVerChangeType.None)]
        public void MemberRemovedPopulatesInformationFromParameters(bool isVisible, SemVerChangeType expected)
        {
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>()
                .Set(x => x.IsVisible = isVisible);

            var actual = ComparisonResult.MemberRemoved(oldMember);

            actual.Should().NotBeNull();

            _output.WriteLine(actual.Message);

            actual.ChangeType.Should().Be(expected);
            actual.OldMember.Should().Be(oldMember);
            actual.NewMember.Should().BeNull();
            actual.Message.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void MemberRemovedThrowsExceptionWithNullOldMember()
        {
            Action action = () => ComparisonResult.MemberRemoved(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NoChangeReturnsValuesFromParameters()
        {
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var newMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var match = new MemberMatch(oldMember, newMember);

            var actual = ComparisonResult.NoChange(match);

            actual.Should().NotBeNull();

            _output.WriteLine(actual.Message);

            actual.ChangeType.Should().Be(SemVerChangeType.None);
            actual.OldMember.Should().Be(oldMember);
            actual.NewMember.Should().Be(newMember);
            actual.Message.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void NoChangeThrowsExceptionWithNullMatch()
        {
            Action action = () => ComparisonResult.NoChange(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}