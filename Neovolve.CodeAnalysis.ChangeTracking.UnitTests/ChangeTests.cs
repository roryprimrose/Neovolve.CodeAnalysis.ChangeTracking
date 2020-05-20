namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;
    using Xunit.Abstractions;

    public class ChangeTests
    {
        private readonly ITestOutputHelper _output;

        public ChangeTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(true, ChangeType.Feature)]
        [InlineData(false, ChangeType.None)]
        public void MemberAddedPopulatesInformationFromParameters(bool isPublic, ChangeType expected)
        {
            var newMember = Model.Create<PropertyDefinition>().Set(x => x.IsPublic = isPublic);

            var actual = Change.MemberAdded(newMember);

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
            Action action = () => Change.MemberAdded(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(ChangeType.Feature)]
        [InlineData(ChangeType.Breaking)]
        public void MemberChangedPopulatesInformationFromParameters(ChangeType changeType)
        {
            var newMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var message = Guid.NewGuid().ToString();

            var actual = Change.MemberChanged(changeType, oldMember, newMember, message);

            actual.Should().NotBeNull();
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

            Action action = () => Change.MemberChanged(ChangeType.Feature, oldMember, newMember, message);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void MemberChangedThrowsExceptionWithNoneChangeType()
        {
            var newMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var message = Guid.NewGuid().ToString();

            Action action = () => Change.MemberChanged(ChangeType.None, oldMember, newMember, message);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void MemberChangedThrowsExceptionWithNullNewMember()
        {
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var message = Guid.NewGuid().ToString();

            Action action = () => Change.MemberChanged(ChangeType.Feature, oldMember, null!, message);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MemberChangedThrowsExceptionWithNullOldMember()
        {
            var newMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var message = Guid.NewGuid().ToString();

            Action action = () => Change.MemberChanged(ChangeType.Feature, null!, newMember, message);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(true, ChangeType.Breaking)]
        [InlineData(false, ChangeType.None)]
        public void MemberRemovedPopulatesInformationFromParameters(bool isPublic, ChangeType expected)
        {
            var oldMember = Model.Create<PropertyDefinition>().Set(x => x.IsPublic = isPublic);

            var actual = Change.MemberRemoved(oldMember);

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
            Action action = () => Change.MemberRemoved(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}