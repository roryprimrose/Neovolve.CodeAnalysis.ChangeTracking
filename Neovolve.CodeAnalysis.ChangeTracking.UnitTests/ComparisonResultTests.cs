﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
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
        public void MemberAddedPopulatesInformationFromParameters(bool isPublic, SemVerChangeType expected)
        {
            var newMember = Model.Create<PropertyDefinition>().Set(x => x.IsPublic = isPublic);

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
            var message = Guid.NewGuid().ToString();

            var actual = ComparisonResult.MemberChanged(changeType, oldMember, newMember, message);

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

            Action action = () =>
                ComparisonResult.MemberChanged(SemVerChangeType.Feature, oldMember, newMember, message);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void MemberChangedThrowsExceptionWithNoneChangeType()
        {
            var newMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var message = Guid.NewGuid().ToString();

            Action action = () => ComparisonResult.MemberChanged(SemVerChangeType.None, oldMember, newMember, message);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void MemberChangedThrowsExceptionWithNullNewMember()
        {
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var message = Guid.NewGuid().ToString();

            Action action = () => ComparisonResult.MemberChanged(SemVerChangeType.Feature, oldMember, null!, message);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MemberChangedThrowsExceptionWithNullOldMember()
        {
            var newMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();
            var message = Guid.NewGuid().ToString();

            Action action = () => ComparisonResult.MemberChanged(SemVerChangeType.Feature, null!, newMember, message);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(true, SemVerChangeType.Breaking)]
        [InlineData(false, SemVerChangeType.None)]
        public void MemberRemovedPopulatesInformationFromParameters(bool isPublic, SemVerChangeType expected)
        {
            var oldMember = Model.Create<PropertyDefinition>().Set(x => x.IsPublic = isPublic);

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
            var newMember = Model.Create<PropertyDefinition>();

            var actual = ComparisonResult.NoChange(oldMember, newMember);

            actual.Should().NotBeNull();

            _output.WriteLine(actual.Message);

            actual.ChangeType.Should().Be(SemVerChangeType.None);
            actual.OldMember.Should().Be(oldMember);
            actual.NewMember.Should().Be(newMember);
            actual.Message.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void NoChangeThrowsExceptionWithNullNewMember()
        {
            var oldMember = Model.UsingModule<ConfigurationModule>().Create<PropertyDefinition>();

            Action action = () => ComparisonResult.NoChange(oldMember, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NoChangeThrowsExceptionWithNullOldMember()
        {
            var newMember = Model.Create<PropertyDefinition>();

            Action action = () => ComparisonResult.NoChange(null!, newMember);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}