namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class ComparisonResultTests
    {
        private readonly ITestOutputHelper _output;

        public ComparisonResultTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CanCreateWithNullNewItem()
        {
            var message = Guid.NewGuid().ToString();
            var changeType = Model.UsingModule<ConfigurationModule>().Create<SemVerChangeType>();

            var oldItem = Substitute.For<IPropertyDefinition>();

            var actual = new ComparisonResult(changeType, oldItem, null!, message);

            actual.Should().NotBeNull();

            _output.WriteLine(actual.Message);

            actual.ChangeType.Should().Be(changeType);
            actual.OldItem.Should().Be(oldItem);
            actual.NewItem.Should().BeNull();
            actual.Message.Should().Be(message);
        }

        [Fact]
        public void CanCreateWithNullOldItem()
        {
            var message = Guid.NewGuid().ToString();
            var changeType = Model.UsingModule<ConfigurationModule>().Create<SemVerChangeType>();

            var newItem = Substitute.For<IPropertyDefinition>();

            var actual = new ComparisonResult(changeType, null!, newItem, message);

            actual.Should().NotBeNull();

            _output.WriteLine(actual.Message);

            actual.ChangeType.Should().Be(changeType);
            actual.OldItem.Should().BeNull();
            actual.NewItem.Should().Be(newItem);
            actual.Message.Should().Be(message);
        }

        [Theory]
        [InlineData(SemVerChangeType.None)]
        [InlineData(SemVerChangeType.Feature)]
        [InlineData(SemVerChangeType.Breaking)]
        public void PropertiesReturnParameterValues(SemVerChangeType changeType)
        {
            var message = Guid.NewGuid().ToString();

            var newItem = Substitute.For<IPropertyDefinition>();
            var oldItem = Substitute.For<IPropertyDefinition>();

            var actual = new ComparisonResult(changeType, oldItem, newItem, message);

            actual.Should().NotBeNull();

            _output.WriteLine(actual.Message);

            actual.ChangeType.Should().Be(changeType);
            actual.OldItem.Should().Be(oldItem);
            actual.NewItem.Should().Be(newItem);
            actual.Message.Should().Be(message);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullMessage()
        {
            var changeType = Model.UsingModule<ConfigurationModule>().Create<SemVerChangeType>();

            var newItem = Substitute.For<IPropertyDefinition>();
            var oldItem = Substitute.For<IPropertyDefinition>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ComparisonResult(changeType, oldItem, newItem, null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}